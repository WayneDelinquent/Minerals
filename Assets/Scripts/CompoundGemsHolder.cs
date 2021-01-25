using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompoundGemsHolder : MonoBehaviour
{
    [SerializeField] GemSelector GemHolder;

	private List<CompoundTower> towers = new List<CompoundTower>();

    public void AddCompoundTower(CompoundTower tower)
	{
		towers.Add(tower);
		GemSelector holder = Instantiate(GemHolder, transform);

		Image image = holder.GetComponent<Image>();
		if (image)
		{
			image.sprite = tower.GetComponent<Tower>().GetSprite();
		}
		holder.SetActiveTowers(tower.GetChosenTowers());
		holder.tower = tower.GetComponent<Tower>();

		var xPosition = (towers.Count - 1) * 150;
		holder.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPosition, 0, 0);
	}
}
