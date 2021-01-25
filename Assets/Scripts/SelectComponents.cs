using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectComponents : MonoBehaviour
{
    [SerializeField] Text selectText;
    public CompoundTower selectedTower;
    public List<Tower> chosenTowers = new List<Tower>();

    private List<Tower> requiredTowers;
    private List<Tower> allTowers;

    public bool isSelectingTower = false;

    public void StartSelectionProcess(CompoundTower tower)
	{
        selectedTower = tower;
        requiredTowers = new List<Tower>(selectedTower.GetComponentTowers());
        allTowers = FindObjectsOfType<Tower>().ToList();

        RunSelectLoop();
	}

    public void ComponentSelected(Tower tower)
	{
        chosenTowers.Add(tower);
        tower.ShowChosen();
        requiredTowers.RemoveAt(0);
        isSelectingTower = false;
        ClearHighlights();

        RunSelectLoop();
    }

    private void RunSelectLoop()
    {
        while (requiredTowers.Count > 0 && !isSelectingTower)
        {
            var available = allTowers.Where(t => t._towerName == requiredTowers[0]._towerName).ToList();
            if (available.Count == 1)
            {
                chosenTowers.Add(available[0]);
                available[0].ShowChosen();
                requiredTowers.RemoveAt(0);
            }
            else
            {
                isSelectingTower = true;
                selectText.text = "Select tower for: " + requiredTowers[0]._towerName;
                foreach (Tower t in available)
                {
                    t.ShowHighlight();
                }
            }
        }

        if (requiredTowers.Count == 0)
		{
            selectText.text = "Select Tower Location (Red Sparks)";
		}
    }

    private void ClearHighlights()
	{
        foreach (Tower t in allTowers)
		{
            t.HideHighlight();
		}
	}


    public void ClearChosen()
	{
        foreach (Tower t in allTowers)
        {
            t.HideChosen();
        }
    }

    public void ClearTowers()
	{
        allTowers.Clear();
	}
}
