using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public float lifeTime;
    public bool isUnActive;
    public damageHitbox DamageHitbox;
	// Use this for initialization
    IEnumerator dealy()
    {
        yield return new WaitForSeconds(lifeTime);
        if(DamageHitbox != null)
        DamageHitbox.selfDestory();
    }
	void Start () {
        if (!isUnActive) StartCoroutine(dealy());
        
	}
	
}
