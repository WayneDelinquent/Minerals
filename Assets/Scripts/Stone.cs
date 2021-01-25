using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    [SerializeField] List<Tower> availableTowers;
    [SerializeField] float _buildSpeed = 1.5f;
    [SerializeField] Slider _slider;
    
    private float _buildRate = 0f;
    private bool _isBuilding = false;
    private Tower _incomingTower;

    // Start is called before the first frame update
    void Start()
    {
        _buildRate = 0f;
        _slider.value = _buildRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBuilding)
		{
            _buildRate += Time.deltaTime / _buildSpeed;
            _slider.value = _buildRate;
            if (_buildRate >= 1f)
			{
                BuildTower();
            }
		}
    }

    public void TransformStone()
    {
        _isBuilding = true;
        DetermineTower();
    }

    public void HideHealthBar()
	{
        _slider.gameObject.SetActive(false);
	}

    private void DetermineTower()
	{
        int index = Random.Range(0, 10000) % availableTowers.Count;
        Tower tower = Instantiate(availableTowers[index]);
        tower.transform.position = transform.position;
        tower.transform.rotation = transform.rotation;
        tower.transform.parent = transform.parent;
        tower.gameObject.name.Replace("(Clone)", "");
        _incomingTower = tower;
        _incomingTower.gameObject.SetActive(false);
        FindObjectOfType<LevelController>().TowerConstructed(tower);
    }

    private void BuildTower()
    {
        gameObject.SetActive(false);
        FindObjectOfType<LevelController>().TowerCompleted(_incomingTower);
        _incomingTower.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
