using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class MyJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public int MovementRange = 40;
        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal";
        public string verticalAxisName = "Vertical";

        Vector3 m_StartPos;
        Vector3 m_StartPos_costum;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        public GameObject mainBody;
        bool isMouseDown = false;
        void OnEnable()
        {
            CreateVirtualAxes();
        }

        void Start()
        {
            m_StartPos = transform.position;
        }

        void UpdateVirtualAxes(Vector3 value,Vector3 start_pos)
        {
            var delta = start_pos - value;
            delta.y = -delta.y;
            delta /= MovementRange;
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(-delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }

            //NetworkClientUI.SendJoystickInfo(-delta.x, delta.y);
           // Debug.Log("delta|"+(-delta.x)+","+ delta.y);
            MyClient.SendJoystickInfo(-delta.x, delta.y);
        }

        void CreateVirtualAxes()
        {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX)
            {
                m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;
            
            if (m_UseX)
            {
                int delta = (int)(data.position.x - m_StartPos.x);
                delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
                newPos.x = delta;
            }

            if (m_UseY)
            {
                int delta = (int)(data.position.y - m_StartPos.y);
                delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
                newPos.y = delta;
            }
          
            transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
            UpdateVirtualAxes(transform.position,m_StartPos);
        }

        public void costumeDrag(Vector3 data)
        {
            Vector3 newPos = Vector3.zero;

            if (m_UseX)
            {
                int delta = (int)(data.x - m_StartPos_costum.x);
                delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
                newPos.x = delta;
            }

            if (m_UseY)
            {
                int delta = (int)(data.y - m_StartPos_costum.y);
                delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
                newPos.y = delta;
            }

            transform.position = new Vector3(m_StartPos_costum.x + newPos.x, m_StartPos_costum.y + newPos.y, m_StartPos_costum.z + newPos.z);
            UpdateVirtualAxes(transform.position,m_StartPos_costum);
        }

        public void setCostumeDrag(Vector3 pos)
        {
            m_StartPos_costum = pos;
        }

        public void OnPointerUp(PointerEventData data)
        {
            transform.position = m_StartPos;
            UpdateVirtualAxes(m_StartPos,m_StartPos);
            isMouseDown = false;
        }

        public void endDrag()
        {
 
            UpdateVirtualAxes(Vector3.zero, Vector3.zero);
        }

        public void OnPointerDown(PointerEventData data)
        {
            isMouseDown = true;
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }

        public void movePad(Vector3 newpos)
        {
            mainBody.transform.position = newpos;
            m_StartPos = newpos;
        }

        public void touchArea()
        {
            if (!isMouseDown)
            {
                Debug.Log(Input.mousePosition);
                movePad(Input.mousePosition);
                isMouseDown = true;
            }

        }

        public void unTouchArea()
        {
          
                isMouseDown = false;

        }
    }
}