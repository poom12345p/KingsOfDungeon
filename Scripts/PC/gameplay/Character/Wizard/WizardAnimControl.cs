using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAnimControl : MonoBehaviour {
    public WizardControl wizardControl;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void fireSpell()
    {
        wizardControl.fireMagic();
    }
    public void endAct()
    {
        wizardControl.endAct();
    }

    public void PlaySound(AudioClip clip){
       /* wizardControl.attackAudioSource.clip = clip;
        wizardControl.attackAudioSource.Play();*/
        wizardControl.attackAudioSource.PlayOneShot(clip);
    }
}
