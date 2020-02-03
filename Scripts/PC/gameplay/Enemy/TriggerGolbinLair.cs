using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGolbinLair : MonoBehaviour {

	public GameObject goblinTent;
	public bool alreadySpawned=false;
	public Transform spwnPos;

	// Use this for initialization
	private void OnTriggerEnter(Collider other)
    {
        CharecterControl cc = other.GetComponent<CharecterControl>();
        if(cc != null && !alreadySpawned && other.tag == "Player")
        {
            GameObject spwn = Instantiate(goblinTent, spwnPos.position, Quaternion.identity);
			spwn.transform.parent = transform;
			alreadySpawned = true;
        }
    }
}
