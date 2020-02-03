using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainingItemWrapper : MonoBehaviour {

	public bool startDraining = false;
	public DropItem item;

	private void OnTriggerEnter(Collider other) {
		//if(item == null) return;
		if(other.tag == "Player" && item.CanPlayerPickItem(other.GetComponent<unitHitbox>()) && !startDraining)
		{
			item.StartCoroutine(item.moveToPicker(other.transform));
			startDraining = true;
		}
	}
}
