using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerMaskEffect : MonoBehaviour, itemEffect, afterHeal, afterTakenDamage {

	unitHitbox owner;

	public int itemCharge = 0;

	[Tooltip("0.5 = if health lost 50%, will increase attack 0.5*50 = 25%")]
	public float percentIncreasePerHeath = 0.5f;
	float currentAttackIncreasePercent;
	float currentDamageIncrease = 0;

	// Get heal
    public void doActionAH()
    {
        calculateNewDamage();
    }

	// Take damage
    public void doActionATD(int damageTaken, unitHitbox takenFrom)
    {
        calculateNewDamage();
    }

    public void GetMoreItem()
    {
        itemCharge++;
		currentAttackIncreasePercent = percentIncreasePerHeath*itemCharge;
		calculateNewDamage();
    }

	void calculateNewDamage()
	{
		owner.statContol.RemoveIncreaseAttackDamage(currentDamageIncrease);
		currentDamageIncrease = ((owner.hpMax - owner.hpCurrent) / (float)owner.hpMax) * currentAttackIncreasePercent;
		owner.statContol.IncreaseAttackDamage(currentDamageIncrease);
	}

    public void RemoveEffect()
    {
        itemCharge--;
		if(itemCharge <= 0)
		{
			owner.statContol.RemoveIncreaseAttackDamage(currentDamageIncrease);
			owner.removeAfterTakenDamage(this);
			owner.removeAfterHeal(this);
			Destroy(gameObject);
		}
		else
		{
			calculateNewDamage();
		}
    }

    public void SetOwnerInstance(unitHitbox unit)
    {
        owner = unit;
		transform.position = owner.transform.position;
		owner.addAfterHeal(this);
		owner.addAfterTakenDamage(this);
		GetMoreItem();
    }
}
