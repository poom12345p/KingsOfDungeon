using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiberoRootItem : ItemBase {

	public RootBombControl rootBombControlInstance;

    public override void doActionAPI(unitHitbox unitHitbox){
		base.doActionAPI(unitHitbox);
		RootBombControl rbc = unitHitbox.GetComponentInChildren<RootBombControl>();
		if(rbc == null)
		{
			rbc = Instantiate(rootBombControlInstance, unitHitbox.transform, false);
			rbc.SetOwnerInstance(unitHitbox);
		}else
		{
			rbc.GetMoreItem();
		}
		
	}

	public override void removeItemEffectOnPlayer(unitHitbox player){
		base.removeItemEffectOnPlayer(player);
		RootBombControl rbc = player.GetComponentInChildren<RootBombControl>();
		if(rbc == null) Debug.LogError("Can't remove no "+itemModel.itemName+" on player");
		else rbc.RemoveEffect();
		
	}
}
