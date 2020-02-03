using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAnimControl : MonoBehaviour
{
    public ArcherControl ac;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void endShoot()
    {
        ac.endShoot();
    }

    public void endDodge()
    {
        ac.endDodge();

    }

    public void shootArrowAct()
    {
        ac.shootArrowAct();

    }
    
    public void playSound(AudioClip ad)
    {
        ac.PlaySound(ad);
    }

    public void deActiveCol()
    { 
        ac.deActiveCol();
    }
    public void activeCol()
    {
        ac.activeCol();
    }

    public void shotPinArrow()
    {
        ac.shootPinArow();
    }
}
