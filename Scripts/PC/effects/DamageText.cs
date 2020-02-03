using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour {
    public Animator anim;
    public Text myText;
	// Use this for initialization
	void Start () {
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
	}
	
    public void setDamageText(string dam)
    {
        myText.text = dam;

    }
}
                                                           