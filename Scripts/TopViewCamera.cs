using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TopViewCamera : MonoBehaviour
{

    public CinemachineVirtualCamera topviewCam;

    [SerializeField]
    private int priorityCameraMax = 100, priorityCameraMin = 5;
    public bool topViewIsTriggered = false;

    private void Start()
    {
        topviewCam.Priority = priorityCameraMin;
        // check follow and look at target
        if (topviewCam.m_LookAt == null || topviewCam.m_Follow == null)
            Debug.LogError("Error Topview Camera Don't have look at and follow target");
    }

    public void ActivateTopView()
    {
        topviewCam.Priority = priorityCameraMax;
        topViewIsTriggered = true;
    }

    public void DeactivateTopView()
    {
        topviewCam.Priority = priorityCameraMin;
        topViewIsTriggered = false;
    }
}
