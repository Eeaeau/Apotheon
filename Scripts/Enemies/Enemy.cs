using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] private Attribute.Element _element;
	[Export] private bool _transferElementOnDeath = true;
	[Export] private float _maxHealth = 100;
	[Export] private float _moveSpeed = 4f;
	[Export] private Node3D _target; // Assign the player node in the editor
	[Export] private NavigationAgent3D _agent; // Now exported for explicit assignment
	[Export] private bool _debug = false; // Enable/disable debug prints

	private float _currentHealth;
	//A matrix of damage multipliers based on elements in the format of [damageElement, entityElement]
	private float[,] _elementDamageMatrix =
	{
		{1f, 2f, 0.5f}, //Water damage
		{0.5f, 1f, 2f}, //Fire damage
		{2f, 0.5f, 1f} //Earth damage
	};

	public override void _Ready()
	{
		_currentHealth = _maxHealth;
		// Fallback: if not assigned in inspector, try to find a direct child named NavigationAgent3D
		if (_agent == null)
		{
			_agent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
			if (_debug) GD.Print($"{Name}: Fallback agent lookup result = {_agent != null}");
		}
		if (_agent == null)
		{
			GD.PushWarning($"Enemy {Name} has no NavigationAgent3D assigned or found as child.");
		}
		else
		{
			_agent.PathDesiredDistance = 0.2f;
			_agent.TargetDesiredDistance = 0.5f;
			if (_debug) GD.Print($"{Name}: Agent configured. PathDesiredDistance={_agent.PathDesiredDistance}, TargetDesiredDistance={_agent.TargetDesiredDistance}");
		}
		if (_debug && _target == null)
		{
			GD.Print($"{Name}: No target assigned yet.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_agent == null)
		{
			if (_debug && (Engine.GetFramesDrawn() % 30) == 0) GD.Print($"{Name}: Skipping movement - no agent.");
			return;
		}
		if (_target == null)
		{
			if (_debug && (Engine.GetFramesDrawn() % 30) == 0) GD.Print($"{Name}: Skipping movement - no target.");
			return;
		}

		// Continuously update target position (player may move)
		_agent.TargetPosition = _target.GlobalTransform.Origin;

		if (_agent.IsNavigationFinished())
		{
			if (_debug && (Engine.GetFramesDrawn() % 30) == 0) GD.Print($"{Name}: Navigation finished. Current pos={GlobalTransform.Origin}, target={_agent.TargetPosition}");
			Velocity = Velocity.MoveToward(Vector3.Zero, (float)delta * _moveSpeed * 2f);
			MoveAndSlide();
			_agent.SetVelocity(Velocity);
			return;
		}

		Vector3[] path = _agent.GetCurrentNavigationPath();
		if (_debug && (Engine.GetFramesDrawn() % 90) == 0)
		{
			GD.Print($"{Name}: Path length={path.Length}, Agent pos={GlobalTransform.Origin}, Target={_agent.TargetPosition}");
		}
		if (path.Length == 0)
		{
			if (_debug && (Engine.GetFramesDrawn() % 30) == 0) GD.Print($"{Name}: Empty path returned. Check navigation map / layers / mesh.");
			return; // Nothing to follow
		}

		Vector3 nextPathPoint = _agent.GetNextPathPosition();
		// Skip first waypoint if it's basically the same horizontal position (often directly below due to height difference)
		Vector3 horizDelta = new Vector3(nextPathPoint.X - GlobalTransform.Origin.X, 0, nextPathPoint.Z - GlobalTransform.Origin.Z);
		if (horizDelta.Length() < 0.05f && path.Length > 1)
		{
			nextPathPoint = path[1];
			if (_debug) GD.Print($"{Name}: Skipping vertical-only waypoint. Using {nextPathPoint}");
		}

		Vector3 toPoint = nextPathPoint - GlobalTransform.Origin;
		Vector3 moveDir = new Vector3(toPoint.X, 0, toPoint.Z);

		if (_debug && (Engine.GetFramesDrawn() % 30) == 0)
		{
			GD.Print($"{Name}: Next point={nextPathPoint}, horizDist={moveDir.Length():F2}");
		}

		if (moveDir.Length() > 0.01f)
		{
			Vector3 desiredVel = moveDir.Normalized() * _moveSpeed;
			Velocity = new Vector3(desiredVel.X, Velocity.Y, desiredVel.Z);
		}
		else
		{
			Velocity = new Vector3(0, Velocity.Y, 0);
		}

		MoveAndSlide();
		_agent.SetVelocity(Velocity); // Helps avoidance update
		if (_debug && (Engine.GetFramesDrawn() % 30) == 0)
		{
			GD.Print($"{Name}: Velocity={Velocity}");
		}
	}

	public void GetHurt(float damage, Attribute.Element damageElement)
	{
		float resultingDamage = damage * _elementDamageMatrix[(int)damageElement, (int)_element];
		_currentHealth -= resultingDamage;
		if (_debug) GD.Print($"{Name}: Damage calc -> base {damage} * mult {_elementDamageMatrix[(int)damageElement, (int)_element]} = {resultingDamage}");

		GD.Print($"{this.Name} hurt for {resultingDamage} damage by a {damageElement} weapon. {_currentHealth} health remaining");
		if (_currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		GD.Print($"{this.Name} perishes");
		TransferElementOnDeath();
		QueueFree();
	}

	private void TransferElementOnDeath()
	{
		if (!_transferElementOnDeath) return;

		GD.Print($"Thansferring {_element} element to the player");

		//Clunky, but there should always be only one player object on the scene, so it should be fine for now
		var players = GetTree().GetNodesInGroup("Player");
		foreach (Player player in players)
		{
			player.ReceiveWeaponElement(_element);
		}
	}

}
