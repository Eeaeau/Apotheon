using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponEffect : Node3D
{		
	[Export]
	private Godot.Collections.Dictionary<Attribute.Element, Node3D> Effects { get; set; }

	public Attribute.Element Element { get { return _element; } set { ChangeElement(value); } }

	private Attribute.Element _element;

	public void ChangeElement(Attribute.Element newElement)
	{
		// Effects[_element].Hide();
		// Effects[newElement].Show();
		// _element = newElement;

	}
}
