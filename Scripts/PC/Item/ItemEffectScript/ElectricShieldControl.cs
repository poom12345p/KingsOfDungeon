using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShieldControl : MonoBehaviour, itemEffect {

	unitHitbox owner;
	int charge = 0;
	public int damagePerCharge = 1;
	public float peroid = 1.5f;

	ContinueDamage continueDamage;

	private void Awake() {
		continueDamage = GetComponentInChildren<ContinueDamage>();
	}

	public void SetOwnerInstance(unitHitbox unit){
		owner = unit;
		transform.position = unit.transform.position;
		continueDamage.DamageHitbox.setOwner(unit);
		continueDamage.damagePeriod = peroid;
		GetMoreItem();
	}

	public void GetMoreItem(){
		charge += 1;
		SetDamagePerCharge();
	}

	void SetDamagePerCharge(){
		continueDamage.DamageHitbox.setDamage(damagePerCharge * charge);
	}

	public void RemoveEffect(){
		charge -= 1;
		if(charge <= 0) Destroy(gameObject);
		else SetDamagePerCharge();
	}
}
