using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractArea : MonoBehaviour {

    [SerializeField] List<InteractiveTrigger> triggersList = new List<InteractiveTrigger>();
    public string areaType;
    Collider collider;

    public UnityEvent afterUP;
    public UnityEvent afterSHAKE;
    public UnityEvent afterDOWN;
    public UnityEvent triggerIn;
    public UnityEvent triggerOut;
    // Use this for initialization
    void Start () {
        collider = GetComponent<Collider>();
        if (!collider) Debug.Log("InteractArea need to assign Collider");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        var trigger = other.GetComponent<InteractiveTrigger>();
        if(trigger)
        {
            trigger.AddInteract(this);
            if(!triggersList.Contains(trigger))
            {
                triggersList.Add(trigger);
                triggerIn.Invoke();
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var trigger = other.GetComponent<InteractiveTrigger>();
        if (trigger)
        {
            trigger.RemoveInteract(this);
            if (triggersList.Contains(trigger))
            {
                triggersList.Remove(trigger);
                triggerOut.Invoke();
            }
        }
    }

    public void GetInteract(InteractType IT)
    {
        switch(IT)
        {
            case InteractType.SHAKE:
                afterSHAKE.Invoke();
                break;
            case InteractType.UP:
                afterUP.Invoke();
        
                break;
            case InteractType.DOWN:
                afterDOWN.Invoke();
       
                break;
        }

    }

    public void DisabledArea()
    {
        collider.enabled = false;
        foreach (var intt in triggersList)
        {
            intt.RemoveInteract(this);
        }
        
    }

    public int GetTriggerCount()
    {
        return triggersList.Count;
    }



 
}
