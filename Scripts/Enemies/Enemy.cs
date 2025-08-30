using Godot;
using System;

public partial class Enemy : CharacterBody3D
{

	public void GetHurt(float damage)
	{
		GD.Print($"{this.Name} hurt for {damage} damage");
	}
}
