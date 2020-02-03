using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrap_Trigger : MonoBehaviour {
	public GameObject trap;
	public bool triggered = false;
	private int playerCount = 0;
	private void Start()
	{
		trap.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !triggered)
		{
			StopAllCoroutines();
			playerCount++;
			trap.SetActive(true);
			trap.GetComponent<Animator>().Play("Trap_WallTrap");
			triggered = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			playerCount--;
			if (playerCount <= 0 && triggered)
			{
				StartCoroutine("cooldown");
			}
		}
	}

	IEnumerator cooldown()
	{
		yield return new WaitForSeconds(5);
		trap.SetActive(false);
		triggered = false;
	}
}
