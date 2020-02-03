using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;
namespace UnityStandardAssets.CrossPlatformInput
{
    public class ArcherBowArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Transform analogBase;
        public Transform analogPin;
        public Image[] analogImg;
        Vector3 startPos;
        public int MovementRange;
        int curTouch;
        int touchAnalogIndex = -1, touchOnAnalogCount = 0;
        // Use this for initialization
        void Start()
        {
            foreach (Image img in analogImg)
                img.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerUp(PointerEventData data)
        {
            MyClient.SendButtonInfo("CHARGERELEASE");
            foreach (Image img in analogImg)
                img.enabled = false;
        }


        public void OnPointerDown(PointerEventData data)
        {

            startPos = data.position;
            analogBase.position = startPos;
            foreach (Image img in analogImg)
                img.enabled = true;

        }


        public void OnDrag(PointerEventData data)
        {
            analogPin.position = data.position;
            /*float angle = Vector3.Angle(analogPin.position - analogBase.position, Vector3.up);
            if (analogPin.position.x > analogBase.position.x)
                angle = -angle + 360;*/
            // Debug.Log(test.phase);
            //Debug.Log(data.position);
            //joyStick.costumeDrag(Input.GetTouch(touchAnalogIndex).position);

        }
    }
}