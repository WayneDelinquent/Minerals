using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private float _originalSpeed = 0f;
    private float _speedCountdown = 0f;
    private float _speedModifier = 0f;

    private Coroutine _currentPoison;
    private float _poisonCountdown = 0f;
    private float _poisonPerSecond = 0f;

    private float _armorCountdown = 0f;
    private float _armorModifier = 0f;

    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpeed();
        CheckArmor();
    }

    public void ApplyStatusEffects(SpecialAttack special)
    {
        if (special == null) return;
        ApplySpeedModifier(special);
        ApplyArmorModifier(special);
        ApplyPoison(special);
    }

    private void ApplyPoison(SpecialAttack special)
	{
        if (_currentPoison == null)
		{
            _currentPoison = StartCoroutine(StartPoison(special.poisonPerSecond, special.poisonDuration));
		} else
		{
            _poisonCountdown = Mathf.Max(_poisonCountdown, special.poisonDuration);
		}
	}

    private IEnumerator StartPoison(float dps, float duration)
	{
        _poisonCountdown = duration;
        while (_poisonCountdown > 0)
		{
            enemy.TakePoisonDamage(dps);
            yield return new WaitForSeconds(1);
            _poisonCountdown--;
		}

        Debug.Log("Poison over");
        _currentPoison = null;
	}

    private void CheckSpeed()
    {
        if (_speedCountdown > 0)
        {
            _speedCountdown = Mathf.Max(_speedCountdown - Time.deltaTime, 0f);
            if (_speedCountdown <= 0)
            {
                enemy.SetSpeed(_originalSpeed);
                _originalSpeed = 0f;
            }
        }
    }

    private void ApplySpeedModifier(SpecialAttack special)
    {
        if (_speedCountdown > 0 && special.speedModifier == _speedModifier)
        {
            _speedCountdown = Mathf.Max(_speedCountdown, special.speedDuration);
        }
        else if (special.speedDuration > 0f && _speedCountdown <= 0f)
        {
            if (_originalSpeed == 0f) _originalSpeed = enemy.GetSpeed();

            _speedCountdown = special.speedDuration;
            _speedModifier = special.speedModifier;
            enemy.SetSpeed(_originalSpeed * special.speedModifier);
        }

    }

    private void CheckArmor()
    {
        if (_armorCountdown > 0)
        {
            _armorCountdown = Mathf.Max(_armorCountdown - Time.deltaTime, 0f);
            if (_armorCountdown <= 0)
            {
                enemy.SetArmorModifier(1f);
            }
        }
    }

    private void ApplyArmorModifier(SpecialAttack special)
    {
        if (_armorCountdown > 0 && special.armorModifier == _armorModifier)
        {
            _armorCountdown = Mathf.Max(_armorCountdown, special.armorDuration);
        }
        else if (special.armorDuration > 0f && _armorCountdown <= 0f)
        {
            _armorCountdown = special.armorDuration;
            _armorModifier = special.armorModifier;
            enemy.SetArmorModifier(special.armorModifier);
        }

    }
}
