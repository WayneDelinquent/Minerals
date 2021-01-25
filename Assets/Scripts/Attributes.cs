using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
	[Header("Base Attributes")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _armor;
    [SerializeField] private int _dexterity;

	[Header("Elemental Resistances")]
    [SerializeField] private float _fireResistance;
    [SerializeField] private float _coldResistance;
    [SerializeField] private float _posionResistance;

	private List<int> dexBracketCeiling;
	private List<float> dexBracketProportion;
	private int _currentHealth;

	private float _armorModifier = 1f;

	public float TakeDamage(float damage, DamageType damageType)
	{
		int rounded = Mathf.Max(Mathf.RoundToInt(damage), 1);
		switch (damageType)
		{
			case DamageType.POISON:
				_currentHealth = Mathf.Max(_currentHealth - rounded, 0);
				return damage;
			default:
				damage = GetAfterArmorDamage(Mathf.RoundToInt(damage));
				rounded = Mathf.Max(Mathf.RoundToInt(damage), 1);
				_currentHealth = Mathf.Max(_currentHealth - rounded, 0);
				return damage;
		}
	}

	public bool IsDead()
	{
		return _currentHealth <= 0;
	}

	public float GetHealthPercentage()
	{
		return (float)_currentHealth / (float)_maxHealth;
	}

	private void Start()
	{
		dexBracketCeiling = LevelController.dexBracketCeiling;
		dexBracketProportion = LevelController.dexBracketProportion;
		_currentHealth = _maxHealth;
	}

	#region Armor
	public int GetAfterArmorDamage(int damage)
	{
		// We can tweak and play with the calculation later.
		// But this scales with higher levels/damage at least
		float damageMultiplier = damage / (float)(damage + GetTotalArmor());
		return Mathf.RoundToInt(damage * damageMultiplier);
	}

	public float GetTotalArmor()
	{
		float totalArmor = _armor * _armorModifier;
		return totalArmor;
	}

	public void SetArmorModifier(float modifier)
	{
		_armorModifier = modifier;
	}
	#endregion

	#region Dexterity
	public bool DoesDodgeAttack()
	{
		float dodgePercent = CalculatePercentageFromDex(GetTotalDodgeDex());

		int dodgeInteger = (int)(dodgePercent * 100);
		int dodgeCheck = Random.Range(0, 10000);

		return dodgeCheck < dodgeInteger;
	}

	public bool IsCriticalHit()
	{
		float critPercent = CalculatePercentageFromDex(GetTotalCritDex());

		int critInteger = (int)(critPercent * 100);
		int critCheck = Random.Range(0, 10000);

		return critCheck < critInteger;
	}

	public bool DoesAttackHit()
	{
		float hitPercent = CalculatePercentageFromDex(GetTotalCritDex());

		int hitInteger = (int)(hitPercent * 100);
		int hitCheck = Random.Range(0, 10000);

		return hitCheck > hitInteger;
	}

	private float CalculatePercentageFromDex(float dexToCalculate)
	{
		float percentage = 0f;
		int index = 0;
		while ((dexToCalculate > 0) && index < Mathf.Min(dexBracketCeiling.Count, dexBracketProportion.Count))
		{
			int bracketRange = index == 0
				? dexBracketCeiling[index]
				: dexBracketCeiling[index] - dexBracketCeiling[index - 1];

			int currentBracketDex = (int)Mathf.Min(bracketRange, dexToCalculate);
			percentage += dexBracketProportion[index] * currentBracketDex;
			dexToCalculate -= currentBracketDex;

			index++;
		}

		if (dexToCalculate > 0)
		{
			percentage += dexBracketProportion[index - 1] * dexToCalculate;
		}

		return Mathf.Min(percentage, 95f);
	}

	public float GetTotalDodgeDex()
	{
		//This will eventually take dex from items/buffs into account
		return _dexterity;
	}

	public float GetTotalCritDex()
	{
		//This will eventually take dex from items/buffs into account
		return _dexterity;
	}
	#endregion
}

public enum DamageType
{
    PHYSICAL, FIRE, COLD, POISON
}

public enum AttackState
{
	HIT, MISSED, CRIT, DODGED
}
