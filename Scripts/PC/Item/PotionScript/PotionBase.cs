using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePotion : MonoBehaviour, afterPickItem
{

    public DropItem dropItem;
    public float delayDestory = 30f;
    public bool nerverDestroy = false;

    // Use this for initialization
    void Start()
    {
        dropItem.addAfterPickItems(this);

        // destroy it if no one pick this
        if (!nerverDestroy)
            Destroy(gameObject, delayDestory);
    }


    public virtual void doActionAPI(unitHitbox player)
    {
        // Debug.Log("player picked "+ gameObject);
    }

    public virtual bool checkPickItemCondition(unitHitbox picker)
    {
        return true;
    }
}