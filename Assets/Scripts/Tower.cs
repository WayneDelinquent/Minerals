using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] public string _towerIdentifier;
    [SerializeField] public string _towerName;

    [SerializeField] float _rangeRadius;

    [Header("Damage Statistics")]
    [SerializeField] int _minDamage;
    [SerializeField] int _maxDamage;
    [SerializeField] float _critMultiplier;

    [Header("Attack Statistics")]
    [Tooltip("Attacks per second")]
    [SerializeField] float _attackRate;
    [SerializeField] Projectile _projectile;
    [SerializeField] float _projectileSpeed;
    [SerializeField] Transform _projectileSpawnLocation;
    [SerializeField] int _maximumTargets = 1;

    [Header("Miscellaneous")]
    [SerializeField] Sprite _towerSprite;
    [SerializeField] ParticleSystem _selectParticle;
    [SerializeField] ParticleSystem _highlightParticle;
    [SerializeField] ParticleSystem _chosenParticle;

    private List<Enemy> _targets = new List<Enemy>();
    private SphereCollider _rc;
    private Attributes _attr;

    public bool isSelected = false;
    public bool isHighlighted = false;
    public bool isChosen = false;

    // Start is called before the first frame update
    void Start()
    {
        _rc = GetComponent<SphereCollider>();
        _rc.radius = _rangeRadius;
        _attr = GetComponent<Attributes>();
    }

	private void Update()
	{
    }

	public Sprite GetSprite()
	{
        return _towerSprite;
    }

    public void ShowHighlight()
    {
        _highlightParticle.gameObject.SetActive(true);
        isHighlighted = true;
    }

    public void HideHighlight()
    {
        _highlightParticle.gameObject.SetActive(false);
        isHighlighted = false;
    }

    public void ShowChosen()
    {
        _chosenParticle.gameObject.SetActive(true);
        isChosen = true;
    }

    public void HideChosen()
    {
        _chosenParticle.gameObject.SetActive(false);
        isChosen = false;
    }

    public void OnTowerClicked()
	{
        if (isHighlighted)
        {
            FindObjectOfType<SelectComponents>().ComponentSelected(this);
        }
        else if (isChosen)
        {
            SelectComponents _sc = FindObjectOfType<SelectComponents>();
            FindObjectOfType<LevelController>().PlaceCompoundTower(this, _sc.selectedTower.GetComponent<Tower>());
            _sc.ClearChosen();
            _sc.ClearTowers();
        }
        else if (FindObjectOfType<SelectComponents>().isSelectingTower)
		{
            Debug.Log("Can't select now");
		} else 
		{
            Tower[] towers = FindObjectsOfType<Tower>();
            foreach (Tower tower in towers)
            {
                tower.UnselectTower();
            }
            SelectTower();
        }
	}

    public void SelectTower()
	{
        isSelected = true;
        _selectParticle.gameObject.SetActive(true);
    }

    public void UnselectTower()
	{
        isSelected = false;
        _selectParticle.gameObject.SetActive(false);
    }

    public void ReplaceWithStone(Stone _stonePrefab)
    {
        Stone stone = Instantiate(_stonePrefab);
        stone.transform.position = transform.position;
        stone.transform.rotation = transform.rotation;
        stone.transform.parent = transform.parent;
        stone.gameObject.name.Replace("(Clone)", "");
        stone.HideHealthBar();
        Destroy(gameObject);
    }

    public void ReplaceWithTower(Tower selectedTower)
    {
        Tower tower = Instantiate(selectedTower);
        tower.transform.position = transform.position;
        tower.transform.rotation = transform.rotation;
        tower.transform.parent = transform.parent;
        tower.gameObject.name.Replace("(Clone)", "");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        if (_targets.Contains(enemy)) return;
        _targets.Add(enemy);
        if (_targets.Count > 1) return;
        StartCoroutine(StartAttacking());
	}

	private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        _targets.Remove(enemy);
    }

    private IEnumerator StartAttacking()
	{
        while (_targets.Count > 0)
		{
            List<Enemy> enemies = DetermineTargets();
            if (enemies != null && enemies.Count > 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    Shoot(enemy);
                }
            }
            yield return new WaitForSeconds(1f / _attackRate);
		}
	}

    private List<Enemy> DetermineTargets()
	{
        RemoveDeadTargets();
        if (_targets.Count <= 0) return null;

        return _targets.GetRange(0, Mathf.Min(_maximumTargets, _targets.Count));
	}

    private void RemoveDeadTargets()
	{
        _targets.RemoveAll((Enemy obj) => obj.IsDead());
	}

    private void Shoot(Enemy enemy)
	{
        Projectile projectile = Instantiate(_projectile, _projectileSpawnLocation.transform.position, transform.rotation) as Projectile;
        projectile.FireAt(this, enemy, _projectileSpeed);
    }

    public KeyValuePair<AttackState, float> DetermineDamage()
	{
        float damage = Random.Range(_minDamage, _maxDamage);
        AttackState state = AttackState.HIT;
        if (!_attr.DoesAttackHit())
		{
            state = AttackState.MISSED;
		}
        else if (_attr.IsCriticalHit())
		{
            damage = damage * _critMultiplier;
            state = AttackState.CRIT;
        }
        return new KeyValuePair<AttackState, float>(state, damage);
    }
}
