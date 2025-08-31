using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export]
	private Attribute.Element _element;
	[Export]
	private float _maxHealth = 100;


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
	}

	public void GetHurt(float damage, Attribute.Element damageElement)
	{
		float resultingDamage = damage * _elementDamageMatrix[(int)damageElement, (int)_element];
		_currentHealth -= resultingDamage;

		GD.Print($"{this.Name} hurt for {resultingDamage} damage by a {damageElement} weapon. {_currentHealth} health remaining");
		if (_currentHealth <= 0)
		{
			Die();
		}		
	}

	private void Die()
	{
		GD.Print($"{this.Name} perishes");
		QueueFree();
	}

}
