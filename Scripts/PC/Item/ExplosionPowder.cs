using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPowder : ItemBase
{

    public BombChainAttackControl bombChainAttackControl;

    public override void doActionAPI(unitHitbox unitHitbox)
    {
        base.doActionAPI(unitHitbox);
        BombChainAttackControl bac = unitHitbox.GetComponentInChildren<BombChainAttackControl>();
        if (bac == null)
        {
            bac = Instantiate(bombChainAttackControl, unitHitbox.transform, false);
            bac.SetOwnerInstance(unitHitbox);
        }
        else
        {
            bac.GetMoreItem();
        }
    }

    public override void removeItemEffectOnPlayer(unitHitbox player)
    {
        base.removeItemEffectOnPlayer(player);
        BombChainAttackControl bac = player.GetComponentInChildren<BombChainAttackControl>();
        if (bac == null) Debug.LogError("Can't remove no " + itemModel.itemName + " on player");
        else bac.RemoveEffect();
    }
}
