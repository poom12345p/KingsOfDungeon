using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAnimation : MonoBehaviour {
	public GameObject sign;
	public bool CompletePlay = false;
	public GameObject hitbox;
	private void Start()
	{
		sign.SetActive(false);
		hitbox.SetActive(true);
	}
	void openDamageHitbox()
	{
		hitbox.SetActive(true);
	}

	void closeDamageHitbox()
	{
		hitbox.SetActive(false);
	}

	void openSign()
	{
		sign.SetActive(true);
	}

	void closeSign()
	{
		sign.SetActive(false);
		CompletePlay = true;
	}
}
