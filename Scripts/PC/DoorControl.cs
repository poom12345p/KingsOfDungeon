using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour {

    Animator _animControl;
    BoxCollider _boxCollider;
    room _roomParent;

    AudioSource _audioSource;
	// Use this for initialization
	void Awake () {
        _animControl = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        _roomParent = transform.parent.parent.GetComponent<room>();
        _audioSource = GetComponent<AudioSource>();
    }
	
	public void CloseTheDoor()
    {
        _animControl.SetBool("closeDoor", true);
        _boxCollider.isTrigger = false;
        //print("Trigger: " + _boxCollider.isTrigger);
    }

    public void OpenTheDoor()
    {
        _animControl.SetBool("closeDoor", false);
        _boxCollider.isTrigger = true;
        //print("Trigger: " + _boxCollider.isTrigger);
    }

    public void PlaySound(AudioClip clip){
        if(_audioSource != null)
        _audioSource.PlayOneShot(clip);
    }
}
