using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node3D
{
	Godot.Collections.Array<Node> enemies;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemies = GetTree().GetNodesInGroup("Enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		enemies = GetTree().GetNodesInGroup("Enemy");

		if (enemies.Count == 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
