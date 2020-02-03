using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NimbleFeather : ItemBase
{

    public float speedUp = 0.1f;

    public override void doActionAPI(unitHitbox unitHitbox)
    {
        base.doActionAPI(unitHitbox);
        // So Ugly, Will change Later
        StatContol stat = unitHitbox.GetComponent<StatContol>();
        stat.IncreaseMaxSpeed(speedUp);
    }

    public override void removeItemEffectOnPlayer(unitHitbox player)
    {
        StatContol stat = player.GetComponent<StatContol>();
        stat.IncreaseMaxSpeed(-speedUp);
    }
}
