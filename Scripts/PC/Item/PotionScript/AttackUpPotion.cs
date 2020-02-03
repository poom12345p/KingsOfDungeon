using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUpPotion : BasePotion
{

    public GameObject AttackUpObject;


    public override void doActionAPI(unitHitbox player)
    {
        // base.doActionAPI(player);
        GameObject attackUp = Instantiate(AttackUpObject, player.transform, false);
        attackUp.GetComponent<AttackUpTemporary>().SetUnitAttackUp(player);
    }


}
