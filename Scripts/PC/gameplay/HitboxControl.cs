using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void hideHitbox(GameObject collider)
    {
        collider.gameObject.GetComponent<Collider>().enabled = false;
    }

    public void activeHitbox(GameObject collider)
    {
        collider.gameObject.GetComponent<Collider>().enabled = true;
    }
}
