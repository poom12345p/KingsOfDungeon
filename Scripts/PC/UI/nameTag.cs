using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nameTag : MonoBehaviour
{
    public Text nametext;
    public Image colortag;

    public void setName(string name)
    {
        this.nametext.text = name;
    }
    public void setColor(Sprite playerColor)
    {
        colortag.sprite = playerColor;
    }

    public void disableColor()
    {
        colortag.gameObject.SetActive(false);
    }

}
