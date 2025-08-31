using Godot;
using System;

public partial class Weapon : Node
{
	[Export]
	public Attribute.Element element;
	[Export]
	public float baseDamage;	
}
