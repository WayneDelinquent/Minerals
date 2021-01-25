using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] List<Level> levels;
    [SerializeField] public int buildingsPerLevel;

    [Header("Dex Bracket Ranges")]
    [Tooltip("Ceiling of current dex bracket to determine dodge percentage")]
    [SerializeField] private List<int> _dexBracketCeiling;
    [Tooltip("Dex levels up until the max of this bracket will add X-percent of dodge")]
    [SerializeField] private List<float> _dexBracketProportion;

    public static List<int> dexBracketCeiling;
    public static List<float> dexBracketProportion;

    [SerializeField] private List<Tower> _towersUnderConstruction = new List<Tower>();
    [SerializeField] private List<GemSelector> _selectableGems;
    [SerializeField] private GameObject _placeGemsButton;
    [SerializeField] private Stone _stonePrefab;
    [SerializeField] private GameObject _gemSelectorPanel;

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _firstWaypoint;

    [Header("CompoundTowers")]
    [SerializeField] private List<CompoundTower> compoundTowers;
    [SerializeField] private CompoundGemsHolder compoundTowerUI;
    [SerializeField] private SelectComponents selectComponents;

    private int currentLevel = 0;
    private int towersCompleted = 0;

    private Level _currentLevel;

    // Start is called before the first frame update
    void Awake()
    {
        dexBracketCeiling = _dexBracketCeiling;
        dexBracketProportion = _dexBracketProportion;
    }

    public void StartNextLevel()
	{
        if (currentLevel + 1 > levels.Count) return;
        currentLevel++;
        _currentLevel = Instantiate(levels[currentLevel - 1]);
        _currentLevel.gameObject.SetActive(true);
        _currentLevel.spawnPoint = _spawnPoint;
        _currentLevel.firstWaypoint = _firstWaypoint;
        _currentLevel.StartLevel();
    }

    public void LevelComplete()
	{
        Destroy(_currentLevel);
        _placeGemsButton.gameObject.SetActive(true);
	}        

    public void TowerSelected(GemSelector gemSelector)
    {
        if (gemSelector.IsCompoundTower())
		{
            SelectCompoundTower(gemSelector);
		} else
        {
            if (gemSelector.HasActiveTowers())
            {
                DiscardTowers(gemSelector);
                SelectSingleTower(gemSelector);
            }
        }
    }

    private void DiscardTowers(GemSelector gemSelector)
	{
        List<Tower> towers = gemSelector.GetActiveTowers();
        foreach (Tower t in _towersUnderConstruction)
        {
            if (!towers.Contains(t)) t.ReplaceWithStone(_stonePrefab);
        }
    }

    private void SelectCompoundTower(GemSelector gemSelector)
    {
        selectComponents = FindObjectOfType<SelectComponents>();
        selectComponents.gameObject.SetActive(true);
        selectComponents.StartSelectionProcess(gemSelector.tower.GetComponent<CompoundTower>());
	}

    private void SelectSingleTower(GemSelector gemSelector)
	{
        List<Tower> towers = gemSelector.GetActiveTowers();
        foreach (Tower t in towers)
        {
            t.HideHighlight();
        }
        ClearGemSelectors();
        _gemSelectorPanel.SetActive(false);
        StartNextLevel();
        _towersUnderConstruction.Clear();
        towersCompleted = 0;
    }

    public void PlaceCompoundTower(Tower towerLocation, Tower selectedPrefab)
	{
        foreach (Tower t in _towersUnderConstruction)
        {
            if (t._towerIdentifier != towerLocation._towerIdentifier) t.ReplaceWithStone(_stonePrefab);
        }

        towerLocation.ReplaceWithTower(selectedPrefab);

        ClearGemSelectors();
        _gemSelectorPanel.SetActive(false);
        StartNextLevel();
        _towersUnderConstruction.Clear();
        towersCompleted = 0;
    }

    public void TowerConstructed(Tower tower)
    {
        tower._towerIdentifier += " " + _towersUnderConstruction.Count;
        _towersUnderConstruction.Add(tower);
        if (_towersUnderConstruction.Count >= 5)
        {
            _placeGemsButton.gameObject.SetActive(false);
        }
    }

    public void TowerCompleted(Tower tower)
    {
        towersCompleted++;
        if (towersCompleted >= 5)
        {
            BuildingsPlaced();
        }
    }

    public void BuildingsPlaced()
    {
        _gemSelectorPanel.SetActive(true);
        List<GemSelector> selectors = new List<GemSelector>(FindObjectsOfType<GemSelector>());
        selectors.Sort((a, b) => a.name.CompareTo(b.name));
        List<Image> images = selectors.Select((selector) => selector.GetComponent<Image>()).ToList();
        for (int i = 0; i < Mathf.Min(images.Count, _towersUnderConstruction.Count); i++)
        {
            images[i].sprite = _towersUnderConstruction[i].GetSprite();
            selectors[i].AddActiveTower(_towersUnderConstruction[i]);
            selectors[i].tower = _towersUnderConstruction[i];
        }

        foreach (CompoundTower tower in compoundTowers)
		{
            if (tower.CanBuildFromTowersUnderConstruction(new List<Tower>(FindObjectsOfType<Tower>())))
            {
                Debug.Log(tower.GetComponent<Tower>()._towerIdentifier + " - CAN be built!!!");
                compoundTowerUI.AddCompoundTower(tower);
            } else
			{
                Debug.Log(tower.GetComponent<Tower>()._towerIdentifier + " - Cannot be built");
			}
        }
    }

    private void ClearGemSelectors()
	{
        List<GemSelector> selectors = new List<GemSelector>(FindObjectsOfType<GemSelector>());
        List<Image> images = selectors.Select((selector) => selector.GetComponent<Image>()).ToList();
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = null;
            selectors[i].ClearActiveTowers();
        }
    }

    // Update is called once per frame
    void Update()
	{
		DetectMouseClick();
	}

	private static void DetectMouseClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("ClickSelector")))
			{
				// whatever tag you are looking for on your game object
				if (hit.collider.gameObject.name == "ClickSelector")
				{
					Tower tower = hit.collider.gameObject.GetComponentInParent<Tower>();
					if (tower)
					{
						tower.OnTowerClicked();
					}
				}
			}
		}
	}
}
