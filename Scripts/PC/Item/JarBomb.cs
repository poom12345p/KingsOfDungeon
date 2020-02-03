using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarBomb : ItemBase
{
    public IceBombControl iceBombControlInstance;

    public override void doActionAPI(unitHitbox unitHitbox)
    {
        base.doActionAPI(unitHitbox);
        IceBombControl icb = unitHitbox.GetComponentInChildren<IceBombControl>();
        if (icb == null)
        {
            icb = Instantiate(iceBombControlInstance, unitHitbox.transform, false);
            icb.SetOwnerInstance(unitHitbox);
        }
        else
        {
            icb.GetMoreItem();
        }
    }

    public override void removeItemEffectOnPlayer(unitHitbox player)
    {
        base.removeItemEffectOnPlayer(player);
        IceBombControl icb = player.GetComponentInChildren<IceBombControl>();
        if (icb == null) Debug.LogError("Can't remove no " + itemModel.itemName + " on player");
        else icb.RemoveEffect();
    }

}

