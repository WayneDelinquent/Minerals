using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Projectile : MonoBehaviour
{
	[SerializeField] DamageType _damageType;

	private Rigidbody _rb;
	private Tower _source;
	private Enemy _target;
	private SpecialAttack _sa;
	private float _speed;

	private void FixedUpdate()
	{
		if (_rb != null)
		{
			_rb.velocity = (_target.gameObject.transform.position - gameObject.transform.position).normalized * _speed;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_target != null && other.gameObject == _target.gameObject)
		{
			HitTarget();
			Destroy(gameObject);
		}
	}

	private void HitTarget()
	{
		KeyValuePair<AttackState, float> hit = _source.DetermineDamage();
		AttackState attackState = hit.Key;
		float damage = hit.Value;

		if (_sa.attackArea > Mathf.Epsilon)
		{
			List<Enemy> enemies = GetAreaTargets();

			foreach (Enemy enemy in enemies)
			{
				enemy.TakeTheHit(damage, _damageType, attackState, _sa);
			}
			
		} else
		{
			_target.TakeTheHit(damage, _damageType, attackState, _sa);
		}
	}

	private List<Enemy> GetAreaTargets()
	{
		List<Enemy> enemies = new List<Enemy>();
		enemies.Add(_target);
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, _sa.attackArea);
		foreach (var hitCollider in hitColliders)
		{
			Enemy enemy = hitCollider.GetComponent<Enemy>();
			if (enemy != null && enemies.Where((e) => e.gameObject.name == enemy.gameObject.name).Count() == 0)
			{
				enemies.Add(enemy);
			}
		}

		return enemies;
	}

	public void FireAt(Tower tower, Enemy enemy, float speed)
    {
		_source = tower;
		_target = enemy;
		_speed = speed;
		_sa = _source.GetComponent<SpecialAttack>();
        _rb = GetComponent<Rigidbody>();
    }
}
