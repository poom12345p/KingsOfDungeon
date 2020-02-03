using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltiPotion : BasePotion
{
    public float ultiRegen;
    public override void doActionAPI(unitHitbox player)
    {
        // base.doActionAPI(player);
        player.ultimateManager.addUlti(ultiRegen);
    }

    public override bool checkPickItemCondition(unitHitbox picker)
    {
        if (picker.ultimateManager.isFull())
        {
            return false;
        }

        return true;
    }
}
