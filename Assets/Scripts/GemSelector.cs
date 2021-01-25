using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSelector : MonoBehaviour
{
    [SerializeField] List<Tower> activeTowers = new List<Tower>();
	public Tower tower;

    public void SetActiveTowers(List<Tower> towers)
	{
        activeTowers = towers;
	}

	public void AddActiveTower(Tower tower)
	{
		activeTowers.Add(tower);
	}

	public void ClearActiveTowers()
	{
		activeTowers.Clear();
	}

	public List<Tower> GetActiveTowers()
	{
		return activeTowers;
	}

	public bool IsCompoundTower()
	{
		return tower.GetComponent<CompoundTower>() != null;
	}

	public bool HasActiveTowers()
	{
		return activeTowers != null && activeTowers.Count > 0;
	}

    public void PointerEnter()
	{
		if (HasActiveTowers())
		{
			foreach (Tower tower in activeTowers)
			{
				tower.ShowHighlight();
			}
		}
	}

	public void PointerExit()
	{
		if (HasActiveTowers())
		{
			foreach (Tower tower in activeTowers)
			{
				tower.HideHighlight();
			}
		}
	}
}
