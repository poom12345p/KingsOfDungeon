using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathTrigger : MonoBehaviour , EnemyControl{

     public UnityEvent afterDeath;
    public AudioSource audioSource;
    public StatContol stat;
    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        stat = GetComponent<StatContol>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public StatContol getCharacterStat()
    {
        return stat;
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        afterDeath.Invoke();
      
    }

    public void selfDestroction(float time)
    {
        Destroy(gameObject, time);
    }
}
