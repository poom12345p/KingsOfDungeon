using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeInstanceControl : MonoBehaviour, afterTakenDamage, itemEffect {

	unitHitbox owner;

	[Range(1,100)]
	public float SawBladeChancePerCharge = 10f;
	float currentChance;

	public float drawbackDamagePercent = 0.3f;

	public int itemCharge;

	public particleControl reflecParticle;

	public void SetOwnerInstance(unitHitbox unit)
	{
		owner = unit;
		transform.position = owner.transform.position;

		owner.addAfterTakenDamage(this);

		GetMoreItem();
	}

	public void GetMoreItem()
    {
        itemCharge++;
        SetCurrentChancePerCharge();
    }

    private void SetCurrentChancePerCharge()
    {
        currentChance = (itemCharge * SawBladeChancePerCharge);
    }

    public void doActionATD(int damage, unitHitbox attacker)
    {
        float randomNum = Random.Range(1f,100f);
		if(randomNum > currentChance) return;

		attacker.takenDamage((int)(damage*drawbackDamagePercent), owner);
		attacker.showParticleEffect(reflecParticle);
    }

	public void RemoveEffect(){
		itemCharge--;
		if(itemCharge <= 0)
		{
			owner.removeAfterTakenDamage(this);
			Destroy(gameObject);
		}
		else SetCurrentChancePerCharge();
	}
}
