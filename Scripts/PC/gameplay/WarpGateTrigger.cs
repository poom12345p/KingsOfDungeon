using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;


public class WarpGateTrigger : MonoBehaviour
{
    public List<unitHitbox> inUnit = new List<unitHitbox>();
    public GuageRing processBar;
    public Text playerCount;
    [SerializeField] private float current = 0, regen;
    [SerializeField] private bool active = true;
    const float max = 50;
    public UnityEvent afterFullEvent;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (inUnit.Count == GameManagerPC.Instance.playerManagement.connectedPlayer)
            {
                current += regen * Time.deltaTime;
            }
            else if (current > 0)
            {
                current -= regen * Time.deltaTime;
                if (current < 0)
                {
                    current = 0;
                }
            }

            if (inUnit.Count > 0)
            {

                if (processBar != null) processBar.updateGauge(max, current);
                if (current >= max)
                {
                    afterFullEvent.Invoke();
                    active = false;
                    current = 100;
                }

            }


        }

    }

    public void regisUnit(unitHitbox unit)
    {
        if (!inUnit.Contains(unit))
        {
            inUnit.Add(unit);
        }

    }

    public void unRegisUnit(unitHitbox unit)
    {
        if (inUnit.Contains(unit))
        {
            inUnit.Remove(unit);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other);
        if (other.GetComponent<unitHitbox>() != null)
        {
            unitHitbox otherHitbox = other.GetComponent<unitHitbox>();

            if (otherHitbox.tag == "Player")
            {
                regisUnit(otherHitbox);
                playerCount.text = inUnit.Count.ToString() + "/" + GameManagerPC.Instance.playerManagement.connectedPlayer.ToString();
                processBar.gameObject.SetActive(true);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<unitHitbox>() != null)
        {
            unitHitbox otherHitbox = other.GetComponent<unitHitbox>();

            if (otherHitbox.tag == "Player")
            {
                unRegisUnit(otherHitbox);
                if (inUnit.Count == 0)
                {
                    processBar.gameObject.SetActive(false);
                }
            }
        }
    }


}
