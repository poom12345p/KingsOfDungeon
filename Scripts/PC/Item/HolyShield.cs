using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyShield : ItemBase 
{
	public ShieldEffectControl shieldEffect;

	public override void doActionAPI(unitHitbox unitHitbox)
	{
		
		base.doActionAPI(unitHitbox);
		ShieldEffectControl shieldEff = unitHitbox.GetComponentInChildren<ShieldEffectControl>();
		if(shieldEff == null )
		{
			shieldEff = Instantiate(shieldEffect, unitHitbox.transform);
			shieldEff.SetOwnerInstance(unitHitbox);
		}else{
			shieldEff.GetMoreItem();
		}
		
	}

	public override void removeItemEffectOnPlayer(unitHitbox player){
		base.removeItemEffectOnPlayer(player);
		ShieldEffectControl shieldEff = player.GetComponentInChildren<ShieldEffectControl>();
		if(shieldEff == null) Debug.LogError("Can't remove no "+itemModel.itemName+" on player");
		else shieldEff.RemoveEffect();
	}
}
