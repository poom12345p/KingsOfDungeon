using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScale : ItemBase {

	public int increaseHp = 10;

	public override void doActionAPI(unitHitbox unitHitbox){
		base.doActionAPI(unitHitbox);

		int upBy = unitHitbox.hpMax + increaseHp;
		unitHitbox.GetComponent<StatContol>().IncreaseAllHp(upBy);
	}

	public override void removeItemEffectOnPlayer(unitHitbox player){
		base.removeItemEffectOnPlayer(player);
		player.GetComponent<StatContol>().IncreaseAllHp(player.hpMax-increaseHp);
	}
}
