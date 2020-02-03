using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerControl : MonoBehaviour
{
    public Pointer[] pointerList = new Pointer[4];
    public Camera currentCam;
    // Use this for initialization
    void Start()
    {
        if (currentCam == null)
            currentCam = Camera.main;
        foreach (var pointer in pointerList)
        {
            pointer.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void checkPosition(int i, Transform pos)
    {
        Vector3 tarScreenPos = currentCam.WorldToScreenPoint(pos.position);
        if (tarScreenPos.x < 0 || tarScreenPos.x > Screen.width || tarScreenPos.y < 0 || tarScreenPos.y > Screen.height)
        {
            pointerList[i].gameObject.SetActive(true);
            pointerList[i].setTarget(pos);
        }
        else
        {
            pointerList[i].gameObject.SetActive(false);
        }
    }
}
