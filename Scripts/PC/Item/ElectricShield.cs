using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShield : ItemBase {

	public ElectricShieldControl electricShieldInstance;

	public override void doActionAPI(unitHitbox player){
		base.doActionAPI(player);
		ElectricShieldControl elc = player.GetComponentInChildren<ElectricShieldControl>();
		if(elc == null){
			elc = Instantiate(electricShieldInstance, player.transform, false);
			elc.SetOwnerInstance(player);
		}else{
			elc.GetMoreItem();
		}
	}

	public override void removeItemEffectOnPlayer(unitHitbox player){
		base.removeItemEffectOnPlayer(player);
		ElectricShieldControl elc = player.GetComponentInChildren<ElectricShieldControl>();
		if(elc == null) Debug.LogError("Can't remove no "+itemModel.itemName+" on player");
		else elc.RemoveEffect();
	} 
}
