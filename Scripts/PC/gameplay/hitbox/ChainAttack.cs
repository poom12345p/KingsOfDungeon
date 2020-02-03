using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAttack : MonoBehaviour,afterDoneDamage {

    // Use this for initialization
    public damageHitbox myHitbox;
    [System.Serializable]
    public struct chainAtk
    {
        public damageHitbox prefabHitbox;
        public damageHitbox model;
        public float delay;
    }
    public chainAtk[] chainList;
    void Start()
    {
        if (myHitbox != null) myHitbox.addAfterDoneDamage(this);
        foreach (chainAtk atk in chainList)
        {
            myHitbox.setDestoryDelay(atk.delay);
        }
    }
	IEnumerator delaySpawn(damageHitbox myModel, damageHitbox myPrefab, Transform spot, unitHitbox attacker,float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnAtk(myModel, myPrefab, spot, attacker);
    }
	// Update is called once per frame
    
	void Update () {
		
	}
    public void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox damageHB)
    {
        foreach(chainAtk atk in chainList)
        {

            StartCoroutine(delaySpawn(atk.model, atk.prefabHitbox, transform, attacker, atk.delay));
           //spawnAtk(atk.model, atk.hitbox, transform, attacker);
        }
    }

    public damageHitbox spawnAtk(damageHitbox myModel, damageHitbox myPrefab, Transform spot, unitHitbox attacker)
    {
        //Debug.Log("spawn: " + myModel);
        if (myModel == null) return null;
        damageHitbox dambox = Instantiate(myPrefab);
        Bullet myBulltet = dambox.GetComponent<Bullet>();
        Bullet modelBullet = myModel.GetComponent<Bullet>();
        if(myBulltet!=null)
        {
            myBulltet.setSpeed(modelBullet.speed);
            myBulltet.setBulletDistance(modelBullet.bulletDistance);
        }
        dambox.transform.position = spot.position;
        dambox.transform.eulerAngles = myModel.transform.eulerAngles;
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(myModel.damage);
        dambox.duplicateAfterDoneDamage(myModel);
        dambox.setOwner(attacker);
        dambox.ultiRegen = myModel.ultiRegen;
        return dambox;
    }
}
