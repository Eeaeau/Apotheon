using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Movement")]
	[Export]
	private float _speed = 5.0f;
	[Export]
	private float _jumpVelocity = 4.5f;

	[ExportGroup("Camera")]
	[Export]
	private float _camSensitivity = 0.006f;

	[ExportSubgroup("Sprinting")]
	[Export]
	private float _camDefaultFOV = 90f;
	[Export]
	private float _camSprintingFOV = 95f;
	[Export]
	private float _fovChangeDuration = 1f;

	[ExportSubgroup("Head bobbing")]
	[Export]
	private float _bobFrequency = 2f;
	[Export]
	private float _bobAmplitude = 0.08f;


	private Node3D _head;
	private Camera3D _cam;

	private bool _isSprinting = false;
	private float _bobTime = 0f;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_head = GetNode<Node3D>("Head");
		_cam = GetNode<Camera3D>("Head/Camera3D");
		_cam.Fov = _camDefaultFOV;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion m)
		{
			_head.RotateY(-m.Relative.X * _camSensitivity);
			_cam.RotateX(-m.Relative.Y * _camSensitivity);

			Vector3 camRot = _cam.Rotation;
			camRot.X = Mathf.Clamp(camRot.X,
				Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
			_cam.Rotation = camRot;
		}

		// exit mouse captured mode with Escape
		if (@event is InputEventKey k && k.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		// check for sprint inputs (on/off)
		if (Input.IsActionJustPressed("sprint"))
		{
			_isSprinting = true;
			Tween t = GetTree().CreateTween();
			t.TweenProperty(_cam, "fov", _camSprintingFOV, _fovChangeDuration)
				.SetEase(Tween.EaseType.InOut)
				.SetTrans(Tween.TransitionType.Cubic);
		}
		else if (Input.IsActionJustReleased("sprint"))
		{
			_isSprinting = false;
			Tween t = GetTree().CreateTween();
			t.TweenProperty(_cam, "fov", _camDefaultFOV, _fovChangeDuration / 2)
				.SetEase(Tween.EaseType.InOut)
				.SetTrans(Tween.TransitionType.Cubic);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = _jumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
		Vector3 direction = (_head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		float speed = _isSprinting ? _speed * 1.4f : _speed;

		if (IsOnFloor())
		{
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * speed;
				velocity.Z = direction.Z * speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, speed * 0.1f);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed * 0.1f);
			}
		}
		else
		{
			velocity.X = Single.Lerp(velocity.X, direction.X * speed, (float)delta * 3f);
			velocity.Z = Single.Lerp(velocity.Z, direction.Z * speed, (float)delta * 3f);
		}
		

		Velocity = velocity;
		MoveAndSlide();

		// Head bobbing
		_bobTime += (float)delta * velocity.Length() * (float)(IsOnFloor() ? 1.0f : 0.0f);
		_cam.Position = HeadBob(_bobTime);
	}

	private Vector3 HeadBob(float time)
	{
		Vector3 bobVector = Vector3.Zero;

		bobVector.Y = (float)Math.Sin(time * _bobFrequency) * _bobAmplitude;
		bobVector.X = (float)Math.Cos(time * _bobFrequency / 2) * _bobAmplitude;

		return bobVector;
	}
}
