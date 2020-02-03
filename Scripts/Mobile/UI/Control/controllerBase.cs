using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class controllerBase : MonoBehaviour {

    [SerializeField] protected bool isInInteractArea;
   [SerializeField] bool isUp;
    // Use this for initialization
    public ControllerFilter controllerFilter;
	void Start () {
		
	}
	
    protected void SpecialInteraction()
    {
        if (!isUp && Input.acceleration.y <= -0.90f)
        {
            isUp = true;
            MyClient.SendButtonInfo("UPINT");
        }
        else if (isUp && Input.acceleration.y > -0.75f)
        {
            MyClient.SendButtonInfo("DOWNINT");
            isUp = false;
        }
        if (Input.acceleration.magnitude >= 2.0f)
        {
            MyClient.SendButtonInfo("SHAKEINT");

        }
    }

    virtual public void receiveMsg(string msg)
    {
        switch (msg)
        {
            case "ININTAREA":
                isInInteractArea = true;
                break;
            case "OUTINTAREA":
                isInInteractArea = false;
                controllerFilter.EndInteract.Invoke();
                break;
            case "MAP":
                controllerFilter.MapInteract.Invoke();
                break;
            case "CHEST":
                controllerFilter.ChestInteract.Invoke();
                Debug.Log("trigeer Chest");
                break;

        }
    }

}
