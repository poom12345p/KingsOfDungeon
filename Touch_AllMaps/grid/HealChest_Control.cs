using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealChest_Control : MonoBehaviour {
	public ChestControl[] AllHealChest;
	public int maxchest = 200;

	List<int> dontSelect;
	// Use this for initialization
	void Start () {
		dontSelect = new List<int>();
		if (maxchest > AllHealChest.Length)
		{
			maxchest = 50;
		}
		randomOpenChest(maxchest);
	}

	void randomOpenChest(int n)
	{
		for (int i = 0; i < AllHealChest.Length - n ; i++)
		{
			int chestIndex = randomInt(dontSelect);
			AllHealChest[chestIndex].gameObject.SetActive(false);
		}
	}

	int randomInt(List<int> d)
	{
		int x = Random.Range(0, AllHealChest.Length - 1);
		while (d.Contains(x))
		{
			x = Random.Range(0, AllHealChest.Length - 1);
		}
		d.Add(x);
		return x;
	}
	
}
