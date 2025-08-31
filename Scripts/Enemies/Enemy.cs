using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export]
	public Attribute.Element element;
	
	//A matrix of damage multipliers based on elements in the format of [damageElement, entityElement]
	private float[,] _elementDamageMatrix =
	{
		{1f, 2f, 0.5f}, //Water damage
		{0.5f, 1f, 2f}, //Fire damage
		{2f, 0.5f, 1f} //Earth damage
	};
	public void GetHurt(float damage, Attribute.Element damageElement)
	{
		float resultingDamage = damage * _elementDamageMatrix[(int)damageElement, (int)element];

		GD.Print($"{this.Name} hurt for {resultingDamage} damage by a {damageElement} weapon");
	}

}
