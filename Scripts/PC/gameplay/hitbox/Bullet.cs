using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour,afterDoneDamage {
    public float speed, bulletDistance;
    public damageHitbox damageHitbox;
    public int maxUnit;
    public bool isActive;
    [SerializeField]
    [Tooltip("bullet's unit capacity(0 for infinity)")]
    private int  unitCount;
    // Use this for initialization 
    void Start () {
        damageHitbox = GetComponent<damageHitbox>();
        damageHitbox.addAfterDoneDamage(this);
       if(isActive) StartCoroutine(lauchArrow());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator lauchArrow()
    {
        float dis = 0;

        while (dis < bulletDistance)
        {
            yield return new WaitForFixedUpdate();
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            dis += speed * Time.deltaTime;
        }
       damageHitbox.deActiveHitbox();
        Destroy(gameObject);
    }
    public void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox dhb)
    {
        
        unitCount++;
        if (unitCount >= maxUnit && isActive&& maxUnit !=0)
        {
            speed = 0;
            damageHitbox.deActiveHitbox();
            Debug.Log("Destroy Arrow");
            damageHitbox.setDestroy();
        }
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public void setBulletDistance(float dis)
    {
        bulletDistance = dis;

    }
}
