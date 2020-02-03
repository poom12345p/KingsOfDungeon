using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUlti : MonoBehaviour {
    
    public Animator anim;
    public ArcherControl archerControl;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void shoot()
    {
        anim.SetTrigger("ATK");
    }
}
