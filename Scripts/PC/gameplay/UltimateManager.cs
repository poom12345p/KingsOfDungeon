using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateManager : MonoBehaviour,afterDoneDamage {
    [Header("Ultimate Guage")]
    public float max;
    public float cur;
    public float regenSpeed;
    [Header("involve Scripts")]
    public GuageRing guageRing;
    public unitHitbox unitHitbox;
    public DamageManager damageManager;
    public ParticleSystem fullChargeParticle;
    public bool isUltimating;
    // Use this for initialization
    void Start () {
        cur = 0;
        guageRing.updateGaugeImediate(max, cur);
        damageHitbox[] DHs = damageManager.GetDamageHitboxes();
        foreach(damageHitbox dh in DHs)
        {

            dh.addAfterDoneDamage(this);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!isFull())
        {
            addUlti(regenSpeed*Time.fixedDeltaTime);
        }
	}

    public void doActionADD(unitHitbox attacker, unitHitbox target,damageHitbox DHB)
    {
        addUlti(DHB.ultiRegen);
    }

    public bool isFull()
    {
        return cur >= max;
    }

    public void resetUltimate()
    {
        cur = 0;
        guageRing.updateGaugeImediate(max, cur);
        if(fullChargeParticle!=null)fullChargeParticle.Stop();
    }

    public void useUltimate()
    {
        resetUltimate();
        isUltimating = true;
    }
    public void endUltimate()
    {
        isUltimating = false;
    }

    public void addUlti(float x)
    {
        if (!isUltimating)
        {
            cur += x;
            if (cur > max)
            {
                cur = max;
               if(fullChargeParticle!=null) fullChargeParticle.Play();
                unitHitbox.ReportUltiFull();
            }
            guageRing.updateGauge(max, cur);
        }
    }

}
