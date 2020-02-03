using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextControl : MonoBehaviour
{

    public DamageText damagePrefabs, healPrefabs, getItemPrefabs;
    public GameObject canvas;
    static public DamageTextControl instance;
    public Camera currentCam;

    // Use this for initialization
    void Start()
    {
        instance = this;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateDamageText(int damage, Transform pos)
    {
        //Debug.Log(pos.position);
        DamageText newDamageText = Instantiate(damagePrefabs);
        newDamageText.transform.SetParent(canvas.transform, false);

        Vector3 screenPos = currentCam.WorldToScreenPoint(pos.position);
        newDamageText.transform.position = screenPos;
        newDamageText.setDamageText(damage.ToString());

        newDamageText.gameObject.SetActive(true);
    }

    public void CreateDamageText(int damage, Transform pos, Color color)
    {
        //Debug.Log(pos.position);
        DamageText newDamageText = Instantiate(damagePrefabs);
        newDamageText.transform.SetParent(canvas.transform, false);
        newDamageText.GetComponentInChildren<Text>().color = color;

        Vector3 screenPos = currentCam.WorldToScreenPoint(pos.position);
        newDamageText.transform.position = screenPos;
        newDamageText.setDamageText(damage.ToString());

        newDamageText.gameObject.SetActive(true);
    }

    public void CreateHealText(int heal, Transform pos)
    {
        // Debug.Log(pos.position);
        DamageText newDamageText = Instantiate(healPrefabs);
        newDamageText.transform.SetParent(canvas.transform, false);

        Vector3 screenPos = currentCam.WorldToScreenPoint(pos.localPosition);
        newDamageText.transform.position = screenPos;
        newDamageText.setDamageText(heal.ToString());

        newDamageText.gameObject.SetActive(true);

    }

    public void CreateGetItemText(string message, Transform pos)
    {
        if (getItemPrefabs != null)
        {
            DamageText newDamageText = Instantiate(getItemPrefabs);
            newDamageText.setDamageText(message);
            newDamageText.transform.SetParent(canvas.transform, false);

            Vector3 screenPos = currentCam.WorldToScreenPoint(pos.position);
            newDamageText.transform.position = screenPos;

            newDamageText.gameObject.SetActive(true);
        }
    }
}
