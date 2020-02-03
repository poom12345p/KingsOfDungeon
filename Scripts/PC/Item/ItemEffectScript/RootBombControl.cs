using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBombControl : MonoBehaviour, afterTakenDamage, itemEffect {

	unitHitbox owner;

	[Range(1,100)]
	public float sproutChancePerCharge = 2.5f;
	float currentSproutChance;
	public int damagePerCharge = 10;
	int currentDamage;

	public GameObject sproutRootBomb;

	public int itemCharge;

	public void SetOwnerInstance(unitHitbox unit)
	{
		owner = unit;
		transform.position = owner.transform.position;

		owner.addAfterTakenDamage(this);
		GetMoreItem();
	}

	public void GetMoreItem(){
		itemCharge++;
		SetDamageAndChacePerCharge();
	}

	void SetDamageAndChacePerCharge(){
		currentSproutChance = (sproutChancePerCharge*itemCharge);
		currentDamage = (damagePerCharge*itemCharge);
	}

    public void doActionATD(int damage, unitHitbox attacker)
    {
		float randomNum = Random.Range(1f,100f);
		if(randomNum > currentSproutChance) return;

		GameObject sprout = Instantiate(sproutRootBomb, transform.position, Quaternion.identity);
		sprout.GetComponent<damageHitbox>().setOwner(owner);
		sprout.GetComponent<damageHitbox>().setDamage(currentDamage);
		sprout.SetActive(true);

    }

	public void RemoveEffect(){
		itemCharge--;
		if(itemCharge <= 0) {
			owner.removeAfterTakenDamage(this); 
			Destroy(gameObject);
		}
		else SetDamageAndChacePerCharge();
	}
}
