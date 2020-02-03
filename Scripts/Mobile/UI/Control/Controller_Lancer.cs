using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Lancer : controllerBase, Controller
{
 
    public float chargeTime;
    public bool isShakeTrigger = false;
    public GameObject ultimateNotify;
    public GameObject ultiBG;
    public bool isUltiFull;
    // Update is called once per frame
    void Start()
    {
        ultimateNotify.SetActive(false);
        ultiBG.SetActive(false);
    }
        void FixedUpdate()
    {

        if (!isInInteractArea)
        {
            if (Input.acceleration.magnitude >= 1.5f)
            {
                //if (!isShakeTrigger)
                //{
                MyClient.SendButtonInfo("ULTISHAKE");
                if(isUltiFull)
                {
                    isUltiFull = false;
                    ultimateNotify.SetActive(false);
                    ultiBG.SetActive(true);

                }
                //isShakeTrigger = true;
                // }
            }
        }
        else
        {
            SpecialInteraction();
        }


    }


    public void pushDown()
    {

        Debug.Log("isHolding: on" );
        MyClient.SendButtonInfo("ATKDOWN");
        //StartCoroutine(chargeUPCount());

    }
    public void releaseAtkBtn()
    {
        Debug.Log("isHolding: off" );
        MyClient.SendButtonInfo("ATKUP");
    }

    override public void receiveMsg(string msg)
    {
        switch (msg)
        {
            case "ULTIFULL":
                isUltiFull = true;
                ultimateNotify.SetActive(true);

                break;
            case "ULTIEND":
                ultiBG.SetActive(false);

                break;
        default:
                base.receiveMsg(msg);
                break;

        }
    }
}
