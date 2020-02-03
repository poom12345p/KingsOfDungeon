using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightUltiControl : MonoBehaviour {
    public Animator anim;
    public damageHitbox DHS, DHL;
    public KnightControl knightControl;
    public float UltiDuration;
    private bool isEnd;
    public enum DHTag
    {
        DH_L,DH_S
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator UltiTineCount()
    {
        yield return new WaitForSeconds(UltiDuration);
        EndUltimate();
        knightControl.sendMsgToClient("ENDULTI");
    }
    public void activeHitbox(DHTag damageTag)
    {
        fitterDH(damageTag).activeHitbox();
    }
    public void hideHitbox(DHTag AtkTag)
    {
        fitterDH(AtkTag).deActiveHitbox();
    }

     public void activeUltimate()
    {
        isEnd = false;
        StartCoroutine("UltiTineCount");
    }
    public void TriggerUltimate()
    {
        if(!isEnd)
        anim.SetTrigger("ATK");
    }
    public void EndUltimate()
    {
        isEnd = true;
        anim.SetTrigger("END");
        knightControl.sendMsgToClient("ENDULTI");
    }
    public void deActiveUltimate()
    {
        knightControl.endUltimate();
    }
    damageHitbox fitterDH(DHTag atkTag)
    {
        damageHitbox myDH;
        switch (atkTag)
        {
        
            case DHTag.DH_L:
                myDH = DHL;
                break;
           case DHTag.DH_S:
                myDH = DHS;
                break;
            default:
                return null;

        }
        return myDH;
    }
}
