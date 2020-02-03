using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.UI;

public class Controller_Archer : controllerBase ,Controller{
    [Header("Analog transform")]
    public Transform analogBase;
    public Transform analogPin;
    public Transform bowPad,bowBase;
    public Transform bowPin;
    [Header("Joystick Setting")]
    public MyJoyStick joyStick;
    public int MovementRange;
    [Header("Analogs Image")]
    public Image[] analogImg,bowImg;
    [Header("Ulti Setting")]
    public GameObject ultimateNotify;
    public GameObject ultimateBG;
    public Transform ultimateArrow;
    public float triggerUltiDistance;
    Vector3 ultiTouchStartPos;
    Vector3 ultiArrowStartPos;
    Vector3 originPos;
    bool isUltiTrigged = false;
    [Space]
    Vector3 startPos,startPosBow;
    bool isBowPinDrag = false, isAnalogPinDrag = false, isReadyChange = false, isTriggerUlti = false, isShakeTrigger = false, isDelay = false;
    [SerializeField] bool isUltiFull=false;
    public Text text;
    public Text[] ttxt = new Text[6];
    
  
    int curTouch;
    Touch test;
   [SerializeField] int touchBowIndex=-1, touchAnalogIndex=-1,touchOnAnalogCount=0, touchOnBowCount = 0;
    const string DRAWBOW = "DRAWBOW", SHOOT = "SHOOT";
    // Use this for initialization
    void Start () {
        ultimateNotify.SetActive(false);
        startPos = bowPin.position;
        /*foreach (Image img in analogImg)
            img.enabled = false;*/
        originPos = bowBase.position;
       /* foreach (Image img in bowImg)
            img.enabled = false;*/
        ultiArrowStartPos = ultimateArrow.transform.position;
        for (int i = 0; i < ttxt.Length; i++)
        {
            ttxt[i] = Instantiate(text);
            ttxt[i].text = i.ToString();
            ttxt[i].transform.SetParent(transform);
        }

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isInInteractArea)
        {
            if (Input.acceleration.x >= -0.10f && Input.acceleration.x <= 0.10f)
            {
                if (!isReadyChange) isReadyChange = true;
            }
            if (Input.acceleration.x >= 0.20f)
            {
                if (isTriggerUlti)
                {
                    MyClient.SendButtonInfo("ULTIRIGHT");
                }
                else if (isReadyChange && Input.acceleration.x >= 0.40f)
                {
                    isReadyChange = false;
                    MyClient.SendButtonInfo("CHANGEMODE");
                }
            }
            else if (Input.acceleration.x <= -0.20f)
            {
                if (isTriggerUlti)
                {

                    MyClient.SendButtonInfo("ULTILEFT");
                }
                else if (isReadyChange && Input.acceleration.x <= -0.40f)
                {
                    isReadyChange = false;
                    MyClient.SendButtonInfo("CHANGEMODERE");
                }
            }

            if (!isDelay && !isShakeTrigger && Input.acceleration.magnitude >= 2.0f)
            {
                Debug.Log("shake");

                if (!isTriggerUlti && isUltiFull)
                    showUltiBow();
                else
                    hideUltiBow();

                isShakeTrigger = true;
                isDelay = true;
                Invoke("delayShake", 0.5f);
            }
            else if (!isDelay && isShakeTrigger && Input.acceleration.magnitude <= 1.2f)
            {
                isShakeTrigger = false;
                isDelay = true;
                Invoke("delayShake", 0.5f);
            }
        }
        else
        {
            SpecialInteraction();
        }

    }

    public void delayShake()
    {
        isDelay = false;
    }

    public void movePin()
    {

        if (!isBowPinDrag)
        {

            float angle = Vector3.Angle(analogPin.position - analogBase.position, Vector3.up);
            if (analogPin.position.x > analogBase.position.x)
                angle = -angle;
            bowPad.eulerAngles = new Vector3(0, 0, angle);
        }

    }
    public void startDrag()
    {
        touchOnAnalogCount++;

       
        if (!isAnalogPinDrag)
        {
            isAnalogPinDrag = true;
            /* touchAnalogIndex = Input.touchCount - 1;
            //int i=0;
            //while (i < Input.touchCount)
            //{
            //    Touch t = Input.GetTouch(i);
            //    if (t.phase == TouchPhase.Began)
            //    {
            //        touchAnalogIndex = i;
            //        break;
            //    }
            //}

            Touch nt = Input.GetTouch(touchAnalogIndex);
            startPos = nt.position;
            analogBase.position = startPos;
            joyStick.setCostumeDrag(startPos);
            foreach (Image img in analogImg)
                img.enabled = true;
          
            //  Debug.Log(Input.touchCount);*/

        }
        if (touchOnAnalogCount >= 2)
        {
            MyClient.SendButtonInfo("DODGE");
            Debug.Log("DODGE");
        }
        Debug.Log("AnaIndex = " + touchAnalogIndex);
    }
    public void stopDrag()
    {
       // Debug.Log("tc|" + Input.touchCount);
        touchOnAnalogCount--;
        //int i = 0;
        //while (i < Input.touchCount)
        //{
        //    Touch t = Input.GetTouch(i);
        //    ttxt[i].transform.position = t.position;
        //    if (t.phase == TouchPhase.Ended)
        //    {
        //        if (i < touchAnalogIndex)
        //        {
        //            touchAnalogIndex--;
        //        }

        //        if (i < touchBowIndex)
        //        {
        //            touchBowIndex--;
        //        }

        //    }
        //    i++;
        //}

        if (touchOnAnalogCount == 0)
        {
            /*foreach (Image img in analogImg)
                img.enabled = false;*/
            isAnalogPinDrag = false;
            analogPin.position = startPos;
            touchAnalogIndex = -1;
            joyStick.endDrag();
            rePosBow();
        }

    }
    public void startDragBow()
    {
        touchOnBowCount++;
   
        
        if (!isBowPinDrag)
        {
            isBowPinDrag = true;
            /* touchBowIndex = Input.touchCount - 1;
             //int i = 0;
             //while (i < Input.touchCount)
             //{
             //    Touch t = Input.GetTouch(i);
             //    if (t.phase == TouchPhase.Began)
             //    {
             //       touchBowIndex = i;
             //        break;
             //    }
             //}
             Touch nt = Input.GetTouch(touchBowIndex);
             startPosBow = nt.position;
             bowBase.position = startPosBow;
             foreach (Image img in bowImg)
                 img.enabled = true;*/
            Debug.Log(DRAWBOW);
            MyClient.SendButtonInfo(DRAWBOW);
            //  Debug.Log(Input.touchCount);

        }
        Debug.Log("BowIndex = " + touchBowIndex);
    }
    void startDragBowPin()
    {
        isBowPinDrag = true;
       /* touchBowIndex = Input.touchCount - 1;
        //int i = 0;
        //while (i < Input.touchCount)
        //{
        //    Touch t = Input.GetTouch(i);
        //    if (t.phase == TouchPhase.Began)
        //    {
        //       touchBowIndex = i;
        //        break;
        //    }
        //}
        Touch nt = Input.GetTouch(touchBowIndex);
        startPosBow = nt.position;
        bowBase.position = startPosBow;
        foreach (Image img in bowImg)
            img.enabled = true;*/
        Debug.Log(DRAWBOW);
        MyClient.SendButtonInfo(DRAWBOW);
    }
    public void stopDragBow()
    {
        touchOnBowCount--;
      /*  int i = 0;
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);
            ttxt[i].transform.position = t.position;
            if (t.phase == TouchPhase.Ended)
            {
                if (i < touchAnalogIndex)
                {
                    touchAnalogIndex--;
                }

                if (i < touchBowIndex)
                {
                    touchBowIndex--;
                }

            }
            i++;
        }*/
    
        if (touchOnBowCount == 0)
        {
            endDragBow();
        }
    }

    void endDragBow()
    {
       /* foreach (Image img in bowImg)
            img.enabled = false;*/
        isBowPinDrag = false;
        bowPin.position = startPosBow;
       // touchBowIndex = -1;
        MyClient.SendButtonInfo(SHOOT);
    }

    public void DragBowPin()
    {
        // Vector3 newPos = Vector3.zero;
        //bowPin.position = Input.GetTouch(touchBowIndex).position;
        float angle = Vector3.Angle(bowPin.position - bowBase.position, Vector3.up);
        if (bowPin.position.x > bowBase.position.x)
            angle = -angle;
        bowPad.eulerAngles = new Vector3(0, 0, angle);
        string msg = "BOWANGLE" + "-" + (angle + 180).ToString();
      //  Debug.Log(msg);
        MyClient.SendButtonInfo(msg);
    }
    public void DragOnMoveArea()
    {
       // Debug.Log(test.phase);
           joyStick.costumeDrag(analogPin.position);
    }

    public void startDragUltiArrow()
    {
        ultiTouchStartPos = Input.GetTouch(0).position;
    }

    public void onDragUltiArrow()
    {
        float dis = Vector3.Distance(ultiTouchStartPos,Input.GetTouch(0).position);
        ultimateArrow.position = ultiArrowStartPos - new Vector3(0, dis, 0);
       // Debug.Log(dis);
        if(dis >=triggerUltiDistance && !isUltiTrigged)
        {
            isUltiTrigged = true;
            MyClient.SendButtonInfo("CHARGEULTI");
        }
        else if(dis < triggerUltiDistance && isUltiTrigged)
        {
            isUltiTrigged = false;
            MyClient.SendButtonInfo("UNCHARGEULTI");

        }
    }

    public void endDragUltiArrow()
    {
        ultimateArrow.position = ultiArrowStartPos;
        MyClient.SendButtonInfo("RELEASESULTI");
        isUltiFull = false;
        ultimateNotify.SetActive(false);
        hideUltiBow();
    }

    public void showUltiBow()
    {
        isTriggerUlti = true;
        ultimateBG.SetActive(true);
        MyClient.SendButtonInfo("ULTI");
    }
    public void hideUltiBow()
    {
        isTriggerUlti = false;
        ultimateBG.SetActive(false);
        MyClient.SendButtonInfo("DEULTI");
    }
    override public void receiveMsg(string msg)
    {
        switch(msg)
        {
            case "ULTIFULL":
                isUltiFull = true;
                ultimateNotify.SetActive(true);
                break;
            default:
                base.receiveMsg(msg);
                break;
        }
    }

    public void rePosAnalog()
    {
        //analogBase.position = originPos;
        //analogPin.position = originPos;

    }

    public void rePosBow()
    {
        bowBase.position = originPos;
        bowPin.position = originPos;
    }

}
