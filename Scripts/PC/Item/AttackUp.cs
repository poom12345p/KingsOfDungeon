using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUp : ItemBase
{
    public float percent = 0.3f;

    public override void doActionAPI(unitHitbox unitHitbox)
    {
        base.doActionAPI(unitHitbox);
        StatContol stat = unitHitbox.GetComponent<StatContol>();
        int newDamage = stat.IncreaseAttackDamage(percent);
        // Debug.Log("Increase Attack To " + newDamage);
    }

    public override void removeItemEffectOnPlayer(unitHitbox player)
    {
        base.removeItemEffectOnPlayer(player);
        StatContol stat = player.GetComponent<StatContol>();
        stat.RemoveIncreaseAttackDamage(percent);
    }
}

