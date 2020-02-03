using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLineAim : MonoBehaviour {

    public Transform target;
    public Transform myLine;
	// Use this for initialization
	void Start () {
        target = null;
	}
	
	// Update is called once per frame
	void Update () {
		if(target!=null)
        {
            myLine.LookAt(target.position);
        }
        else
        {
            myLine.localRotation = Quaternion.identity;
        }

	}
    private void OnTriggerEnter(Collider other)
    {
        if(target == null)
        {
            if(other.tag== "Enemy")
            {
                target = other.transform;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform ==target)
        {

            target = null;
            
        }
    }
}
