using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerTopViewCamera : MonoBehaviour
{
    public TopViewCamera topViewCamera;
    private List<GameObject> playerInside = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
        //playerInside = new List<GameObject>();

        if (topViewCamera == null) Debug.Log("TopViewCamera of TriggerTopview Can't be null!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !playerInside.Contains(other.gameObject))
        {
            playerInside.Add(other.gameObject);
            if (!topViewCamera.topViewIsTriggered)
            {
                topViewCamera.ActivateTopView();
            }

        }
    }

    // Update is called once per frame
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && playerInside.Contains(other.gameObject))
        {
            playerInside.Remove(other.gameObject);
            if (topViewCamera.topViewIsTriggered && playerInside.Count == 0)
            {
                topViewCamera.DeactivateTopView();
            }
        }
    }
}
