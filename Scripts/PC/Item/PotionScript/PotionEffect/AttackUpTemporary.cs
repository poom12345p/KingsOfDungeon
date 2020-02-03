using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUpTemporary : MonoBehaviour
{

    public float attackUpPercent = 0.4f;
    public float lifeTime = 30f;

    unitHitbox owner;

    public void SetUnitAttackUp(unitHitbox unit)
    {
        owner = unit;
        int newAttack = owner.statContol.IncreaseAttackDamage(attackUpPercent);
        StartCoroutine(DelayDestroy());

        // Debug.Log("Increase attack to " + newAttack);
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        owner.statContol.RemoveIncreaseAttackDamage(attackUpPercent);
        Destroy(gameObject);
    }

}
