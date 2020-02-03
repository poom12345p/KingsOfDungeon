using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerMaskItem : ItemBase {

	public BerserkerMaskEffect maskEffect;

    public override void doActionAPI(unitHitbox unitHitbox){
		base.doActionAPI(unitHitbox);
		BerserkerMaskEffect bsm = unitHitbox.GetComponentInChildren<BerserkerMaskEffect>();
		if(bsm == null)
		{
			bsm = Instantiate(maskEffect, unitHitbox.transform, false);
			bsm.SetOwnerInstance(unitHitbox);
		}else
		{
			bsm.GetMoreItem();
		}
	}

	public override void removeItemEffectOnPlayer(unitHitbox player)
	{
		base.removeItemEffectOnPlayer(player);
		BerserkerMaskEffect bsm = player.GetComponentInChildren<BerserkerMaskEffect>();
		if(bsm == null) Debug.LogError("Can't remove no "+itemModel.itemName+" on player");
		else bsm.RemoveEffect();
	}
}
