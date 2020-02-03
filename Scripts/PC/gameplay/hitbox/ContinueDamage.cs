using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueDamage : MonoBehaviour
{
    public damageHitbox DamageHitbox;
    public float damagePeriod;
    public bool unActive;
    // Use this for initialization
    void Start()
    {
        if (!unActive) startBlinking();
    }

    IEnumerator blinking()
    {
        while (true)
        {
            // Debug.Log("DamBlink:"+gameObject.name);
            yield return new WaitForSeconds(damagePeriod);
            DamageHitbox.activeHitbox();
            yield return new WaitForFixedUpdate();
            DamageHitbox.deActiveHitbox();

        }

    }
    public void startBlinking()
    {
        gameObject.SetActive(true);
        DamageHitbox.gameObject.SetActive(true);
        StartCoroutine("blinking");
    }
    public void stopBlinking()
    {
        StopCoroutine("blinking");
    }
}
