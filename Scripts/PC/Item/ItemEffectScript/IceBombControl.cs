using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBombControl : MonoBehaviour, afterDoneDamage, itemEffect
{

    unitHitbox owner;

    [Range(1, 100)]
    public float freezeChangePerCharge = 10f;
    float currentFreezeChance;
    public int damagePerCharge = 10;
    int currentDamage;

    public GameObject freezeBombInstance;

    public int itemCharge;


    public void SetOwnerInstance(unitHitbox unit)
    {
        owner = unit;
        transform.position = owner.transform.position;
        owner.damageManager.addAfterDoneDamageToAll(this);
        GetMoreItem();
    }

    public void GetMoreItem()
    {
        itemCharge++;
        SetDamageAndChacePerCharge();
    }

    void SetDamageAndChacePerCharge()
    {
        currentFreezeChance = (freezeChangePerCharge * itemCharge);
        currentDamage = (damagePerCharge * itemCharge);
    }

    public void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox damageHB)
    {
        float randomNum = Random.Range(1f, 100f);
        if (randomNum > currentFreezeChance) return;

        GameObject freezeBomb = Instantiate(freezeBombInstance, target.transform.position, Quaternion.identity);
        damageHitbox dhb = freezeBomb.GetComponent<damageHitbox>();
        dhb.setOwner(owner);
        dhb.setDamage(currentDamage);
        freezeBomb.SetActive(true);
    }

    public void RemoveEffect()
    {
        itemCharge--;

        if (itemCharge <= 0)
        {
            owner.damageManager.removeAllAfterDoneDamage(this);
            Destroy(gameObject);
        }
        else SetDamageAndChacePerCharge();
    }
}


