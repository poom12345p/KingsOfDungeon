using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller_Wizard :controllerBase ,Controller{

    public bool isUnCasted = false;
    public Transform skillSelect;
    [SerializeField] Vector3[] skillspot;
    private int curSkillIndex=0;
    public GameObject ultimateNotify;
    // Use this for initialization
    void Start () {
        ultimateNotify.SetActive(false);
        skillSelect.localPosition = skillspot[curSkillIndex];

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isInInteractArea)
        {
            if (Input.acceleration.y <= -0.80f)
            {
                if (!isUnCasted)
                {
                    MyClient.SendButtonInfo("UNCAST");
                    isUnCasted = true;
                }
            }
            else if (Input.acceleration.y > -0.45f)
            {
                if (isUnCasted)
                {

                    isUnCasted = false;
                }
            }
            if (Input.acceleration.magnitude >= 1.5f)
            {
                MyClient.SendButtonInfo("ULTIUP");
            }
        }
        else
        {
            SpecialInteraction();
        }
    }


    override public void receiveMsg(string msg)
    {
        switch (msg)
        {
            case "ULTIFULL":
                ultimateNotify.SetActive(true);
                break;
            case "CHARGEUP":
                curSkillIndex = curSkillIndex + 1 > skillspot.Length ? curSkillIndex : curSkillIndex + 1;
                skillSelect.localPosition = skillspot[curSkillIndex];
                break;
            case "RECHARGE":
                curSkillIndex = 0;
               skillSelect.localPosition = skillspot[curSkillIndex];
                break;
            case "ULTIEND":
                ultimateNotify.SetActive(false);

                break;
            default:
                base.receiveMsg(msg);
                break;
        }
    }
}
