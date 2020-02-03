using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerAnimControl : MonoBehaviour {

    public Animator anim;
    public Animator lanceAnim;
    public LancerControl lc;
    public ParticleSystem lanceParticle,rushParticle;
    public AudioSource spearAudio;
    public enum DamboxTag
    {
        normalAtk,rushAtk,Ulti
    }
    [System.Serializable]
    public struct DamboxData
    {
        public DamboxTag tag;
        public damageHitbox DamageHitbox;
    }

    public DamboxData[] damboxData;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   public  void setAnimRun()
    {
        anim.SetInteger("Speed", 2);
        anim.SetBool("isRunning", true);
        anim.ResetTrigger("isATK");

    }
    public void setAnimUnRun()
    {
         anim.SetInteger("Speed", -1);
        if (lc.isDeath) return;
        anim.SetBool("isRunning", false);

       // anim.SetInteger("Speed", -1);

    }
    public void setAnimAtk()
    {
        if (lc.isDeath) return;
        anim.SetTrigger("ATK");
       anim.SetInteger("Speed", 4);
        lanceAnim.SetBool("isATK", true);
    }
    public void setAnimRush()
    {
        if (lc.isDeath) return;
        anim.SetInteger("Speed", 5);
        lanceAnim.SetBool("isATK", true);
    }
    public void setAnimRushUlti()
    {
        if (lc.isDeath) return;
        anim.SetInteger("Speed",7);
        lanceAnim.SetBool("isATK", true);
    }

    public void setAnimUnRush()
    {
        if (lc.isDeath) return;
        anim.SetInteger("Speed", -1);
        lanceAnim.SetBool("isATK", false);
    }

    public void setAnimEndAtk()
    {
        if (lc.isDeath) return;
        //anim.SetInteger("Speed", -1);
        lanceAnim.SetBool("isATK", false);
        anim.ResetTrigger("isATK");
        lc.endATKAct();
    }


    public void UnActiveDamBox(DamboxTag dt)
    {
        foreach (var db in damboxData)
        {
            if(db.tag==dt)
            {
                db.DamageHitbox.deActiveHitbox();
                db.DamageHitbox.gameObject.SetActive(false);
                return;
            }
        }
    }


    public void ActiveDambox(DamboxTag dt)
    {
           foreach (var db in damboxData)
            {
                if (db.tag == dt)
                {
                db.DamageHitbox.gameObject.SetActive(true);
                db.DamageHitbox.activeHitbox();
                    return;
                }
            }
    }

    public void PlayLancePaticle()
    {
        lanceParticle.Play();

            for (int i = 0; i < lc.atkLongFacter ; i++)
            {
                lc.extraAtkparticle[i].Play();
            }
    }
    public void PlayRushPaticle()
    {
        rushParticle.Play();
    }
    public void StopRushPaticle()
    {
        rushParticle.Stop();
    }
    public void RestAnim()
    {
        rushParticle.Stop();
        lanceAnim.SetBool("isATK", false);
       anim.SetInteger("Speed", -1);
        anim.SetBool("isRunning", false);
    }

    public void SetAnimWarp()
    {
        lanceAnim.SetBool("isATK", true);
        anim.SetTrigger("WARP");
       
    }

    public void SetAnimDeath()
    {
        RestAnim();
   
        anim.SetInteger("Speed", 6);
 
    }

    public void playSound(AudioClip ac)
    {
        lc.PlaySound(ac);
    }

    public void playSoundSpear(AudioClip ac)
    {
        spearAudio.PlayOneShot(ac);
    }

    public void DoubleAtkRondom()
    {
        int chance = Random.Range(1, 101);
        if(chance<lc.doubleAtkchance)
        {
            ActiveDambox(DamboxTag.normalAtk);
            lanceParticle.Play();
        }
    }
}
