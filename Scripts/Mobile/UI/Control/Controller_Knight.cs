using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Knight : controllerBase, Controller {
   [SerializeField] bool isBlocking,isHolding, isCharge,isPreAtk;
    public float chargeTime;
    public GameObject ultimateNotify,ultiBG;
    [SerializeField] private bool isUltiFull;
    private void Start()
    {
        ultimateNotify.SetActive(false);
        ultiBG.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (!isInInteractArea)
        {
            if (!isBlocking && Input.acceleration.y <= -0.90f)
            {
                isBlocking = true;
                MyClient.SendButtonInfo("BLOCK");
            }
            else if (isBlocking && Input.acceleration.y > -0.75f)
            {
                isBlocking = false;
                MyClient.SendButtonInfo("UNBLOCK");
            }

            if (Input.acceleration.sqrMagnitude > 3.5f)
            {
                MyClient.SendButtonInfo("ULTIMATE");

                Debug.Log("ULTIMAtE");
                if (isUltiFull)
                {
                    ultimateNotify.SetActive(false);
                    ultiBG.SetActive(true);
                    isUltiFull = false;
                }
            }
        }
        else
        {
            SpecialInteraction();
        }
    }



    public void pushAttack()
    {
        MyClient.SendButtonInfo("ATTACK");
    }

    public void pushDown()
    {

        isHolding = true;
        Debug.Log("isHolding:" + isHolding);
        MyClient.SendButtonInfo("CHARGEUP");
        //StartCoroutine(chargeUPCount());

    }
    public void releaseAtkBtn()
    {
        isHolding = false;
        Debug.Log("isHolding:" + isHolding);
        MyClient.SendButtonInfo("CHARGERELEASE");
    }

    override public void receiveMsg(string msg)
    {
            switch (msg)
            { 
                case "ULTIFULL":
                    ultimateNotify.SetActive(true);
                    isUltiFull = true;
                    break;
                case "ENDULTI":
                    ultimateNotify.SetActive(false);
                ultiBG.SetActive(false);
                break;
            default:
                base.receiveMsg(msg);
                break;

            }
        Debug.Log("msg|" + msg);
    }
}
