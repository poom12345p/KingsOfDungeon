using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{

    public UnityEvent whenPlayerEnter;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) Debug.LogError("TutorialTrigger should have a collider to trigger");
        collider.isTrigger = true; // in case forgot to check isTrigger	
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // print("player enter");
            whenPlayerEnter.Invoke();
        }
    }
}
