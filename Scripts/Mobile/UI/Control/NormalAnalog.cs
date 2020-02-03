using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class NormalAnalog : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,  IDragHandler
    {
        public Transform analogBase;
        public Transform analogPin;
        public Image[] analogImg;
        public MyJoyStick joyStick;
        Vector3 startPos;
        Vector3 OriginPos;
        bool isBowPinDrag = false, isAnalogPinDrag = false;
        public int MovementRange;
        int curTouch;
        int touchAnalogIndex = -1, touchOnAnalogCount = 0;
        // Use this for initialization
        void Start()
        {
            OriginPos = analogBase.transform.position;
           /* foreach (Image img in analogImg)
                img.enabled = false;*/
        }

        // Update is called once per frame
        void Update()
        {
           /* if (curTouch > Input.touchCount)
            {
                Debug.Log("reIndex");
                if (touchAnalogIndex >= Input.touchCount)
                {
                    while (touchAnalogIndex > Input.touchCount - 1)
                    {
                        touchAnalogIndex--;

                    }
                }


            }
            curTouch = Input.touchCount;*/
        }


       public void OnPointerDown(PointerEventData data)
        {
            touchOnAnalogCount++;

            isAnalogPinDrag = true;
            // touchAnalogIndex = Input.touchCount - 1;
            /* Touch nt = Input.GetTouch(touchAnalogIndex);
              startPos = nt.position;
              analogBase.position = startPos;*/
            startPos = data.position;
            analogBase.position = startPos;
            joyStick.setCostumeDrag(startPos);
            foreach (Image img in analogImg)
                img.enabled = true;
            MyClient.SendButtonInfo("ANALOGDOWN");
            //  Debug.Log(Input.touchCount);
        }
        public void OnPointerUp(PointerEventData data)
        {
            // Debug.Log("tc|" + Input.touchCount);
            touchOnAnalogCount--;
            MyClient.SendButtonInfo("ANALOGUP");
       
                /*foreach (Image img in analogImg)
                     img.enabled = false;*/
              
                isAnalogPinDrag = false;
                analogPin.position = startPos;

               // touchAnalogIndex = -1;
                joyStick.endDrag();
                rePostAnalog();
        }

        public void OnDrag(PointerEventData data)
        {
            // Debug.Log(test.phase);
            //Debug.Log(data.position);
            //joyStick.costumeDrag(Input.GetTouch(touchAnalogIndex).position);
            joyStick.costumeDrag(data.position);
        }

        public void rePostAnalog()
        {
            analogBase.position = OriginPos;
            analogPin.position = OriginPos;
        }

    }
}
