using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : ItemBase {

	public SawBladeInstanceControl SawBladeInstance;

    public override void doActionAPI(unitHitbox unitHitbox){
		base.doActionAPI(unitHitbox);
		SawBladeInstanceControl sbc = unitHitbox.GetComponentInChildren<SawBladeInstanceControl>();
		if(sbc == null)
		{
			sbc = Instantiate(SawBladeInstance, unitHitbox.transform, false);
			sbc.SetOwnerInstance(unitHitbox);
		}else
		{
			sbc.GetMoreItem();
		}
	}

	public override void removeItemEffectOnPlayer(unitHitbox player)
	{
		base.removeItemEffectOnPlayer(player);
		SawBladeInstanceControl sbc = player.GetComponentInChildren<SawBladeInstanceControl>();
		if(sbc == null) Debug.LogError("Can't remove no "+itemModel.itemName+" on player");
		else sbc.RemoveEffect();
	}
}
