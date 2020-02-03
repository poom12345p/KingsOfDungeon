using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
    public Image mask, colorTask;
    public Transform arrow, target;
    public Camera currentCam;
    public float speed, xThreshold, yThreshold;


    // Use this for initialization
    void Start()
    {
        if (currentCam == null) currentCam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null) return;
        Vector3 tarScreenPos = currentCam.WorldToScreenPoint(target.position);
        Vector3 targetDir = tarScreenPos - arrow.position;
        // The step size is equal to speed times frame time.
        float angle = Vector3.Angle(tarScreenPos - arrow.position, Vector3.up);
        if (tarScreenPos.x > arrow.position.x)
            angle = -angle + 360;
        arrow.rotation = Quaternion.Lerp(arrow.rotation, Quaternion.Euler(0, 0, angle), Time.time * speed);

        if (tarScreenPos.x > xThreshold && tarScreenPos.x < Screen.width - xThreshold)
        {
            transform.position = new Vector3(tarScreenPos.x, transform.position.y, transform.position.z);

        }
        else if (tarScreenPos.x < xThreshold)
        {
            transform.position = new Vector3(xThreshold, transform.position.y, transform.position.z);
        }
        else if (tarScreenPos.x > Screen.width - xThreshold)
        {
            transform.position = new Vector3(Screen.width - xThreshold, transform.position.y, transform.position.z);
        }


        if (tarScreenPos.y > yThreshold && tarScreenPos.y < Screen.height - yThreshold)
        {
            transform.position = new Vector3(transform.position.x, tarScreenPos.y, transform.position.z);

        }
        else if (tarScreenPos.y < yThreshold)
        {
            transform.position = new Vector3(transform.position.x, yThreshold, transform.position.z);
        }
        else if (tarScreenPos.y > Screen.height - yThreshold)
        {
            transform.position = new Vector3(transform.position.x, Screen.height - yThreshold, transform.position.z);
        }


    }

    public void setTarget(Transform t)
    {
        target = t;
    }

}
