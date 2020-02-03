using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageHitbox : MonoBehaviour
{

    public int damage;
    //public float speed,bulletDistance;
    public bool isFriendly;
    public bool isUnFriendly;
    //public bool isOneTarget,isBullet;
    public particleControl effect;
    Collider myCol;
    public unitHitbox ownerUnit;
    public List<unitHitbox> takenUnit = new List<unitHitbox>();
    public int ultiRegen;
    public bool isDestroyafterDoneDamage = false;
    public float sizeMultifier;
    public float destoryDelay;
    public List<afterDoneDamage> afterDoneDamagesList = new List<afterDoneDamage>();
    public List<beforeDestory> beforeDestoryList = new List<beforeDestory>();
    [Header("for spawning")]
    public damageHitbox myPrefab;

    const int friendlyDamageLayer = 14;
    const int unfriendlyDamageLayer = 15;

    //Collider myCollider;
    // Use this for initialization
    void Awake()
    {

        myCol = gameObject.GetComponent<Collider>();
        /* if (tag != "Damage")
             Debug.Log(name + "need to be taged Damage");*/
        //myCol.enabled = false;
        /*if(isBullet)
        {
            StartCoroutine(lauchArrow());
        }*/
    }
    /*  IEnumerator lauchArrow()
      {
          float dis = 0;

          while(dis<bulletDistance)
          {
              yield return new WaitForFixedUpdate();
              transform.Translate(Vector3.forward * speed * Time.deltaTime);
              dis += speed * Time.deltaTime;
          }
          deActiveHitbox();
      }*/

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if(isBullet)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }*/
    }

    public void regisUnit(unitHitbox unit)
    {
        takenUnit.Add(unit);
        foreach (afterDoneDamage atf in afterDoneDamagesList)
        {
            atf.doActionADD(ownerUnit, unit, this);
        }
        if (isDestroyafterDoneDamage) Invoke("selfDestory", destoryDelay);
        /* if(isOneTarget && takenUnit.Count >=1)
         {
             deActiveHitbox();
         }*/
    }
    public void selfDestory()
    {
        foreach (beforeDestory bd in beforeDestoryList)
        {
            bd.doAction();
        }
        Destroy(gameObject, destoryDelay);
    }
    public bool isDamaged(unitHitbox unit)
    {
        return takenUnit.Contains(unit);
    }

    public void deActiveHitbox()
    {
        takenUnit.Clear();
        //this.gameObject.SetActive(false);
        if (myCol != null) myCol.enabled = false;
        /* if(isBullet)
         {
             Destroy(gameObject);
         }*/
    }


    public void activeHitbox()
    {
        takenUnit.Clear();

        myCol.enabled = true;
    }

    public void setDamage(int damage)
    {
        this.damage = damage;
    }
    public void setUltiRegen(int point)
    {
        this.ultiRegen = point;
    }
    /*public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public void setBulletDistance(float dis)
    {
        bulletDistance = dis;

    }*/

    public void setAngle(Vector3 angle)
    {
        transform.eulerAngles = new Vector3(angle.x, angle.y, angle.z);
    }

    public void addAfterDoneDamage(afterDoneDamage aft)
    {
        if (!afterDoneDamagesList.Contains(aft))
        {
            afterDoneDamagesList.Add(aft);
        }
    }

    public void removeAfterDoneDamage(afterDoneDamage aft)
    {
        if (afterDoneDamagesList.Contains(aft))
        {
            afterDoneDamagesList.Remove(aft);
        }
    }

    public void setOwner(unitHitbox owner)
    {
        this.ownerUnit = owner;
        if (owner != null)
        {
            isFriendly = owner.tag == "Player";
            isUnFriendly = owner.tag == "Enemy";


            if (isUnFriendly)
            {
                if (tag == "Heal")
                {
                    gameObject.layer = friendlyDamageLayer;
                }
                else
                    gameObject.layer = unfriendlyDamageLayer;
            }

            if (isFriendly)
            {
                if (tag == "Heal")
                {
                    gameObject.layer = unfriendlyDamageLayer;
                }
                else
                gameObject.layer = friendlyDamageLayer;
            }
        }
    }

    public void duplicateAfterDoneDamage(damageHitbox dh)
    {
        foreach (afterDoneDamage atf in dh.afterDoneDamagesList)
        {
            afterDoneDamagesList.Add(atf);
        }
    }

    public void setDestroy()
    {
        isDestroyafterDoneDamage = true;
    }
    public void setDestoryDelay(float d)
    {
        if (d > destoryDelay)
        {
            destoryDelay = d + 0.25f;
        }
    }

    public damageHitbox spawnDuplicate(Transform spot)
    {

        damageHitbox dambox = Instantiate(myPrefab);
        Bullet myBulltet = dambox.GetComponent<Bullet>();
        Bullet modelBullet = gameObject.GetComponent<Bullet>();
        dambox.transform.position = spot.position;
        dambox.transform.eulerAngles = spot.transform.eulerAngles;
        dambox.transform.localScale = dambox.transform.localScale * (1 + sizeMultifier);
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(this.damage);
        dambox.duplicateAfterDoneDamage(this);
        dambox.isFriendly = this.isFriendly;
        dambox.isUnFriendly = this.isUnFriendly;
        if (myBulltet != null)
        {
            myBulltet.setSpeed(modelBullet.speed);
            myBulltet.setBulletDistance(modelBullet.bulletDistance);
        }
        dambox.setOwner(this.ownerUnit);
        dambox.ultiRegen = this.ultiRegen;
        return dambox;
    }

    public void spawnDuplicateN(Transform spot)
    {

        spawnDuplicate(spot);
    }
}
