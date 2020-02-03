using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePotion : BasePotion
{
    [SerializeField] int heal;

    public override void doActionAPI(unitHitbox player)
    {
        // base.doActionAPI(player);
        // Debug.Log("Heal");
        player.takenHeal(heal);
    }

    public override bool checkPickItemCondition(unitHitbox picker)
    {
        if (picker.hpCurrent == picker.hpMax)
        {
            return false;
        }

        return true;
    }



}
