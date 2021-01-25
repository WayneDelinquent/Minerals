using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dmgText;
    private Seeker _seeker;
    private AIDestinationSetter _dest;
    private AIPath _path;
    private Attributes _attr;
    private Animator _anim;
    private Rigidbody _rb;
    private string _identifier;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _attr = GetComponent<Attributes>();
        _path = GetComponent<AIPath>();
        _rb = GetComponent<Rigidbody>();
        _dmgText = GetComponentInChildren<TextMeshProUGUI>();
        _dmgText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _dmgText.gameObject.transform.rotation = Quaternion.LookRotation(_dmgText.gameObject.transform.position - Camera.main.transform.position);
    }

    public float GetSpeed()
	{
        return _path.maxSpeed;
	}

    public void SetSpeed(float speed)
	{
        _path.maxSpeed = speed;
	}

    public void SetArmorModifier(float armor)
	{
        _attr.SetArmorModifier(armor);
	}

    public void SetWaypoint(Transform waypoint)
	{
        _seeker = GetComponent<Seeker>();
        _dest = GetComponent<AIDestinationSetter>();
        if (_seeker != null)
        {
            _dest.target = waypoint;
        }
    }

    public float GetHealthPercentage()
	{
        return _attr.GetHealthPercentage();
	}

    public bool IsDead()
	{
        return _attr.IsDead();
	}

    public void StopEnemy()
    {
        _path.maxSpeed = 0f;
    }

    public void TakeTheHit(float damage, DamageType type, AttackState attackState, SpecialAttack special)
    {
        if (attackState != AttackState.MISSED && _attr.DoesDodgeAttack())
        {
            attackState = AttackState.DODGED;
        }

        if (attackState == AttackState.HIT || attackState == AttackState.CRIT)
		{
            StatusEffects effects = GetComponent<StatusEffects>();
            if (effects)
            {
                effects.ApplyStatusEffects(special);
            }

            damage = _attr.TakeDamage(damage, type);
        }
        string text = damage.ToString();

        switch (attackState)
		{
            case AttackState.CRIT:
                _dmgText.color = Color.red;
                text += "!!";
                StartCoroutine(ShowText(text, 1.5f));
                break;
            case AttackState.MISSED:
                _dmgText.color = Color.blue;
                text = "Missed!!";
                StartCoroutine(ShowText(text, 1.5f));
                break;
            case AttackState.DODGED:
                _dmgText.color = Color.blue;
                text = "Dodged!!";
                StartCoroutine(ShowText(text, 1.5f));
                break;
            case AttackState.HIT:
                //_dmgText.color = Color.white;
                break;
        }

        if (_attr.IsDead())
		{
            _anim.SetTrigger("Die");
            StopEnemy();
            Destroy(gameObject, 1.5f);
		}
	}

    public void TakePoisonDamage(float damage)
    {
        damage = _attr.TakeDamage(damage, DamageType.POISON);

        string text = damage.ToString();
        _dmgText.color = Color.green;

        StartCoroutine(ShowText(text, 0.5f));
    }

    public IEnumerator ShowText(string text, float duration)
	{
        _dmgText.gameObject.SetActive(true);
        _dmgText.text = text;
        yield return new WaitForSeconds(duration);
        _dmgText.color = Color.white;
        _dmgText.gameObject.SetActive(false);
	}
}
