using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_dropItem_spawnmons : MonoBehaviour
{
	public int itemChange = 8;// *10 %
	private int chestType;
	public GameObject itemTrigger;
	public GameObject enemyTrigger;
	EnemySpawnTrigger Et;
	DropControl It;
	// Use this for initialization
	void Start()
	{
		//GetComponent
		Et = enemyTrigger.GetComponent<EnemySpawnTrigger>();
		It = itemTrigger.GetComponent<DropControl>();
		//random chest type
		if (Random.Range(0, 10) > itemChange)
		{
			itemTrigger.gameObject.SetActive(false);
			chestType = 2;
		}
		else
		{
			enemyTrigger.gameObject.SetActive(false);
			chestType = 1;
		}

	}


	public void afterOpen()
	{
		switch (chestType)
		{
			case 1:
				//drop item
				It.randomDrop();
				break;
			case 2:
				//enemy spawn
				Et.StartSpawn();
				break;
		}
	}
}
