using Godot;
using System;

public partial class Weapon : Node
{
	[Export]
	public Attribute.Element Element { get { return _element; } set { ChangeElement(value); } }
	[Export]
	public float baseDamage;

	private WeaponEffect _weaponEffect;
	private Attribute.Element _element = Attribute.Element.Fire;

	public override void _Ready()
	{
		_weaponEffect = GetNode<WeaponEffect>("SwordPoint/Effect");

		ChangeElement(_element);
	}

	private void ChangeElement(Attribute.Element newElement)
	{
		_element = newElement;
		_weaponEffect.Element = _element;
	}
}
