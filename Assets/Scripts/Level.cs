using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public Transform firstWaypoint;

    [SerializeField] float _timeBetweenSpawns;
    [SerializeField] Enemy _enemyPrefab;
    [SerializeField] int _enemyCount;

    private bool _allEnemiesSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_allEnemiesSpawned)
		{
            if (GetActiveEnemyCount() <= 0)
			{
                FindObjectOfType<LevelController>().LevelComplete();
			}
		}
    }

    private int GetActiveEnemyCount()
	{
        return FindObjectsOfType<Enemy>().Length;
	}

    public void StartLevel()
    {
        StartCoroutine(SpawnMonsters());
    }

    private IEnumerator SpawnMonsters()
	{
        for (int i = 1; i <= _enemyCount; i++)
        {
            Enemy enemy = Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity) as Enemy;
            enemy.SetWaypoint(firstWaypoint);
            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
        _allEnemiesSpawned = true;
	}
}
