using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowInstantiator : MonoBehaviour, afterDoneDamage
{

    public damageHitbox damageHitbox;

    public SlowInstance slowInstance;

    private void Awake()
    {
        damageHitbox.addAfterDoneDamage(this);
    }

    public void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox damageHB)
    {
        SlowInstance slow = Instantiate(slowInstance, target.transform);
        slow.transform.position = target.transform.position;
        slow.SetSlowTarget(target);
    }
}

