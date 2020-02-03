using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nameTagControl : MonoBehaviour
{
    public nameTag tag_model;
    public Transform canvas;
    public nameTag[] playertag = new nameTag[4];
    public Camera currentCam;

    public Sprite[] colortaglist;

    public void create(int index, string name)
    {
        playertag[index] = Instantiate(tag_model);
        playertag[index].setName(name);
        if (colortaglist.Length > 0 && colortaglist.Length >= index && colortaglist[index] != null)
            playertag[index].setColor(colortaglist[index]);
        else playertag[index].disableColor();
        playertag[index].transform.SetParent(canvas, false);


    }
    public void setNameTagToPlayer(int index, Transform pos)
    {
        if (playertag[index] == null) return;
        Vector3 screenPos = currentCam.WorldToScreenPoint(pos.localPosition);
        playertag[index].transform.position = screenPos;
    }
}
