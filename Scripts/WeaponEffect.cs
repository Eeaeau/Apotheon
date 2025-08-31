using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponEffect : Node3D
{
	private Godot.Collections.Dictionary<Attribute.Element, Node3D> Effects = new Godot.Collections.Dictionary<Attribute.Element, Node3D>();
	public Attribute.Element Element { get { return _element; } set { ChangeElement(value); } }

	private Attribute.Element _element;

	public override void _Ready()
	{
		Effects.Add(Attribute.Element.Water, GetNode<Node3D>("Water"));
		Effects.Add(Attribute.Element.Fire, GetNode<Node3D>("Fire"));
		Effects.Add(Attribute.Element.Earth, GetNode<Node3D>("Earth"));
	}

	public void ChangeElement(Attribute.Element newElement)
	{
		Effects[_element].Hide();
		Effects[newElement].Show();
		_element = newElement;

	}
}
