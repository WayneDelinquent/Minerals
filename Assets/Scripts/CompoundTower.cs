using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CompoundTower : MonoBehaviour
{
    [SerializeField] List<Tower> _components;

	[SerializeField] List<Tower> chosenTowers = new List<Tower>();

    public bool CanBuildFromTowersUnderConstruction(List<Tower> towers)
	{
		chosenTowers.Clear();
		foreach (Tower tower in _components)
		{
			int index = towers.FindIndex(_tower => tower._towerName == _tower._towerName);
			if (index >= 0)
			{
				chosenTowers.Add(towers[index]);
				towers.RemoveAt(index);
				continue;
			}
			return false;
		}
		chosenTowers.Clear();
		return true;
	}

	public List<Tower> GetComponentTowers()
	{
		return _components;
	}

	public List<Tower> GetChosenTowers()
	{
		return chosenTowers;
	}
}
