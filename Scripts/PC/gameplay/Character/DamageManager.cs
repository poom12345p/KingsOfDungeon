using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour {

    [System.Serializable]
    public struct DamageData
    {
        public damageHitbox damageHitbox;
       [HideInInspector] public int damage;
        public int baseDamage;
        [HideInInspector] public int ultiRegen;
        public int baseUltiRegen;
       // public float sizeMultifier;
        public override string ToString(){
            return "damage "+damage+", baseDamage: "+baseDamage;
        }
    }
    unitHitbox unitHitbox;
    public DamageData[] damageDataList;
    public List<extraDamage> extraDamageList =new List<extraDamage>();
    public List<extraUltiRegen> extraUltiRegenlList = new List<extraUltiRegen>();
    private float extraDamagePercent = 0;
    // Use this for initialization
    void Start () {
        unitHitbox = gameObject.GetComponent<unitHitbox>();
        /*baseDamage = new int[damageHitboxList.Count];*/
        for(int i =0;i< damageDataList.Length;i++)
        {
            damageDataList[i].damage = damageDataList[i].baseDamage;
            damageDataList[i].ultiRegen = damageDataList[i].baseUltiRegen;
            damageDataList[i].damageHitbox.setDamage(damageDataList[i].baseDamage);
            damageDataList[i].damageHitbox.setOwner(unitHitbox);
            damageDataList[i].damageHitbox.setUltiRegen(damageDataList[i].ultiRegen);
        }

    }
	
    public void setAllTotalDamage()
    {
        for (int i = 0; i < damageDataList.Length; i++)
        {
            float addition=0;
            foreach(extraDamage addp in extraDamageList)
            {
                addition += addp.getPercent();
            }
            damageDataList[i].damage = damageDataList[i].baseDamage + (int)(((damageDataList[i].baseDamage*addition)/100)+extraDamagePercent);
            damageDataList[i].damageHitbox.setDamage(damageDataList[i].damage);
//            print("after change damage "+damageDataList[i]);

        }
    }
    public void setAllTotalUltiRegen()
    {
        for (int i = 0; i < damageDataList.Length; i++)
        {
            float addition = 0;
            foreach (extraUltiRegen addp in extraUltiRegenlList)
            {
                addition += addp.getPercent();
            }

            damageDataList[i].ultiRegen = damageDataList[i].baseUltiRegen + (int)((damageDataList[i].baseUltiRegen * addition) / 100);
            damageDataList[i].damageHitbox.setUltiRegen(damageDataList[i].ultiRegen);
        }
    }

    public damageHitbox[] GetDamageHitboxes()
    {
        damageHitbox[] DHs=new damageHitbox[damageDataList.Length];
        for(int i=0;i< damageDataList.Length;i++)
        {
            DHs[i] = damageDataList[i].damageHitbox;
        }
        return DHs;
    }

    public void addAfterDoneDamageToAll(afterDoneDamage atf)
    {
        foreach( DamageData dhdt in damageDataList)
        {
            dhdt.damageHitbox.addAfterDoneDamage(atf);
        }
    }

    public void removeAllAfterDoneDamage(afterDoneDamage atf)
    {
        foreach( DamageData dhdt in damageDataList)
        {
            dhdt.damageHitbox.removeAfterDoneDamage(atf);
        }
    }
   

    public void addExtraDamage(extraDamage ed)
    {
        extraDamageList.Add(ed);
        setAllTotalDamage();
    }
    public void plusExtraDamage(float percent)
    {
        if (percent<0)
       {
            Debug.Log("get worg plusExtradamage");
            return;
        }
        extraDamagePercent += percent;
    }
}
