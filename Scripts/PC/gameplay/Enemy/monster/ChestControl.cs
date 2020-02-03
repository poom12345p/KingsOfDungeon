using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class ChestControl : MonoBehaviour, EnemyControl
{
    Animator anim;
    public bool isDeath;
    public UnityEvent AfterOpen;
    public StatContol stat;
    public AudioSource audioSource;
    public ParticleSystem openParticle;
    // Use this for initialization

    void Start () {
        audioSource = GetComponent<AudioSource>();
        stat = GetComponent<StatContol>();
        anim = gameObject.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReportDeath()
    {
        OpenChest();
        isDeath = true;
    }

    public StatContol getCharacterStat()
    {
        return stat;
    }

    public void PlaySound(AudioClip audioClip){
      if(audioSource!=null)  audioSource.PlayOneShot(audioClip);
    }

    public void selfDestruction(float t)
    {
        Destroy( gameObject,t);
    }
    public void OpenChest()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        anim.SetTrigger("OPEN");
        AfterOpen.Invoke();
        openParticle.Play();
    }
}
