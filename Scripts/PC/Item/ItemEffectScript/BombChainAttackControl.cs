using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombChainAttackControl : MonoBehaviour, afterDoneDamage, itemEffect
{

    unitHitbox unitOwner;
    public GameObject Bomb;

    public int bombHitPerCharge = 5;

    int itemCharge;
    int maxitemChange;

    public ItemBase mainItem;


    private void Awake()
    {
        Bomb.SetActive(false);
    }

    public void SetOwnerInstance(unitHitbox unit)
    {
        unitOwner = unit;
        unitOwner.damageManager.addAfterDoneDamageToAll(this);
        maxitemChange = 1;
        itemCharge = bombHitPerCharge;
    }

    public void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox damageHB)
    {
        // bomb damage is 1/3 of attack power
        if (Bomb != null && maxitemChange > 0)
        {
            int bombDamage = unitOwner.damageManager.damageDataList[0].damage / 3;
            damageHitbox damageHb = Bomb.GetComponent<damageHitbox>();
            damageHb.setOwner(unitOwner);
            damageHb.setDamage(bombDamage);

            GameObject BombInstance = Instantiate(Bomb);
            BombInstance.transform.position = target.transform.position;
            BombInstance.SetActive(true);

            itemCharge--;
            if (itemCharge <= 0)
            {
                var unitItem = unitOwner.GetComponent<ItemOnPlayer>();
                if (unitItem != null) unitItem.RemoveItem(mainItem);
            }
        }
    }

    public void GetMoreItem()
    {
        // reset charge
        itemCharge = bombHitPerCharge;
        maxitemChange++;
    }

    public void RemoveEffect()
    {
        maxitemChange--;

        if (maxitemChange <= 0)
        {
            // destroy this gameObject
            unitOwner.damageManager.removeAllAfterDoneDamage(this);
            Destroy(gameObject);
        }
        else
        {
            // reset charge
            itemCharge = bombHitPerCharge;
        }
    }
}
