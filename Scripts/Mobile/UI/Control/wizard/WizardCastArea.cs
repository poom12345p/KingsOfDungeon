using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class WizardCastArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Transform analogBase;
        public Transform analogPin;
        public Image[] analogImg;
        Vector3 startPos;
        Vector3 OriginPos;
        public int MovementRange;
        int curTouch;
        int touchAnalogIndex = -1, touchOnAnalogCount = 0;
        // Use this for initialization
        void Start()
        {
            OriginPos = analogBase.transform.position;
            /*foreach (Image img in analogImg)
                img.enabled = false;*/
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        public void OnPointerUp(PointerEventData data)
        {
            MyClient.SendButtonInfo("CHARGERELEASE");
            rePostAnalog();
           /* foreach (Image img in analogImg)
                img.enabled = false;*/
        }


        public void OnPointerDown(PointerEventData data) {
        
            startPos = data.position;
            analogBase.position = startPos;
           /* foreach (Image img in analogImg)
                img.enabled = true;*/
            MyClient.SendButtonInfo("CHARGEUP");
        }
        //public void startDrag()
        //{
        //    touchOnAnalogCount++;

        //    isAnalogPinDrag = true;
        //    touchAnalogIndex = Input.touchCount - 1;
        //    Touch nt = Input.GetTouch(touchAnalogIndex);
        //    startPos = nt.position;
        //    analogBase.position = startPos;
        //    joyStick.setCostumeDrag(startPos);
        //    foreach (Image img in analogImg)
        //        img.enabled = true;

        //    //  Debug.Log(Input.touchCount);
        //}
        //public void stopDrag()
        //{
        //    // Debug.Log("tc|" + Input.touchCount);
        //    touchOnAnalogCount--;
        //    if (touchOnAnalogCount == 0)
        //    {
        //        foreach (Image img in analogImg)
        //            img.enabled = false;
        //        isAnalogPinDrag = false;
        //        analogPin.position = startPos;
        //        touchAnalogIndex = -1;
        //        joyStick.endDrag();
        //    }
        //}

        public void OnDrag(PointerEventData data)
        {
            analogPin.position = data.position;
            float angle = Vector3.Angle(analogPin.position - analogBase.position, Vector3.up);
            if (analogPin.position.x > analogBase.position.x)
                angle = -angle+360;
            string msg = "CASTANGLE" + "-" + (angle).ToString();
            MyClient.SendButtonInfo(msg);
            // Debug.Log(test.phase);
            //Debug.Log(data.position);
            //joyStick.costumeDrag(Input.GetTouch(touchAnalogIndex).position);

        }
        public void rePostAnalog()
        {
            analogBase.position = OriginPos;
            analogPin.position = OriginPos;
        }

    }
}

