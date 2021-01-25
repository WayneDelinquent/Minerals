using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public Waypoint nextWaypoint;

	private void OnTriggerEnter(Collider other)
	{
		if (nextWaypoint != null)
		{
			Enemy enemy = other.GetComponent<Enemy>();
			if (enemy)
			{
				enemy.SetWaypoint(nextWaypoint.transform);
			}
		}
		else
		{
			Destroy(other.gameObject);
		}
	}
}
