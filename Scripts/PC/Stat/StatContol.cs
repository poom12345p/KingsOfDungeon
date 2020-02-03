using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class StatContol : MonoBehaviour
{

    public DamageManager damageManager;
    public int damage;

    // set hp on unithitbox directly
    private int baseHp;
    public unitHitbox unit;
    //public int hpMax;
    //public int hpCurrent;

    public float speedMax;
    public float speedCurrent;
    public float baseSpeed;
    public float percentUpSpeed;
    public float additionalUpSpeed;

    public NavMeshAgent agent;
    [Header("Increase speed stat")]
    public float speedPerMap;
    public float moreSpeedPerplayer;
    [Header("Increase hp stat")]
    public float hpPerMap;
    public float moreHpPerPlayer = 0.2f;
    [Header("Increase damage stat")]
    public float damagePerMap;
    public float moreDamagePerPlayer = 0.2f;

    bool isPlayer;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        unit = GetComponent<unitHitbox>();
        damageManager = GetComponent<DamageManager>();

        if (unit == null) Debug.LogError("any character should have unitHitbox attach to it");

        baseHp = unit.hpMax;
    }

    private void Start()
    {
        // change stat per map
        if (GameManagerPC.Instance != null && tag == "Enemy")
        {
            GameManagerPC gameManager = GameManagerPC.Instance;
            int level = gameManager.mapLevel > 0 ? gameManager.mapLevel - 1 : 0;
            int connectedPlayer = gameManager.playerManagement.connectedPlayer > 0 ? gameManager.playerManagement.connectedPlayer - 1 : 0;
            StatChangePerMap(level, connectedPlayer);
        }

        if (tag == "Player")
        {
            isPlayer = true;
            PlayerStatChange();
        }

    }

    public void ChangeSpeedNormal(float additionalSpeed)
    {

        float current = speedCurrent;
        additionalUpSpeed += additionalSpeed;
        speedCurrent = baseSpeed + additionalUpSpeed;

        if (speedCurrent < 0)
        {
            speedCurrent = 0;
        }
        else if (speedCurrent > speedMax)
        {
            speedCurrent = speedMax;
        }

        AfterChangeSpped();
    }

    public void ChangeSpeedPercent(float speedPercent)
    {

        percentUpSpeed += speedPercent;
        //Debug.Log("uppercent:" + speedCurrent);
        speedCurrent = baseSpeed + (baseSpeed * percentUpSpeed);
        if (speedCurrent < 0)
        {
            speedCurrent = 0;
        }
        else if (speedCurrent > speedMax)
        {
            speedCurrent = speedMax;
        }
        AfterChangeSpped();

    }

    public void IncreaseMaxSpeed(float speedIncrease)
    {
        baseSpeed += speedIncrease;
        speedMax += speedIncrease;
        speedCurrent += speedIncrease;
        AfterChangeSpped();
    }


    public void IncreaseAllHp(int newMaxHp)
    {
        int incresed = newMaxHp - unit.hpMax;
        unit.hpMax = newMaxHp;
        unit.hpCurrent = unit.hpCurrent + incresed;
        unit.hpRing.updateGauge(unit.hpMax, unit.hpCurrent);

        PlayerStatChange();
    }

    public void StatChangePerMap(int mapLevel, int playerPlaying)
    {
        int newHp = baseHp + (int)((baseHp * (hpPerMap * mapLevel + playerPlaying * moreHpPerPlayer)));
        IncreaseAllHp(newHp);

        int newDamage = 0;
        if (damageManager != null)
        {
            newDamage = IncreaseAttackDamage(mapLevel * damagePerMap + playerPlaying * moreDamagePerPlayer);
            // print("increase damage "+ (mapLevel * damagePerMap));
        }
        else if (damage > 0)
        {
            newDamage = (2 * (int)(mapLevel + playerPlaying * moreDamagePerPlayer));
            damage += newDamage;
            // print("increase damage "+(2*mapLevel));
        }

        float newSpeed = speedPerMap * mapLevel + playerPlaying * moreSpeedPerplayer;
        ChangeSpeedPercent(newSpeed);

        // Debug.Log(gameObject.name + " hp:" + newHp + ", Attack:" + newDamage + "Speed: " + newSpeed);
    }


    public int IncreaseAttackDamage(float percentIncrese)
    {
        AddExtraDamage ed = new AddExtraDamage(percentIncrese);
        damageManager.extraDamageList.Add(ed);
        damageManager.setAllTotalDamage();

        PlayerStatChange();

        return damageManager.damageDataList[0].damage;
    }

    public void RemoveIncreaseAttackDamage(float percentIncrese)
    {
        for (int i = 0; i < damageManager.extraDamageList.Count; i++)
        {
            AddExtraDamage ed = new AddExtraDamage(percentIncrese);
            if (damageManager.extraDamageList[i].Equals(ed))
            {
                damageManager.extraDamageList.RemoveAt(i);
            }
        }
        damageManager.setAllTotalDamage();

        PlayerStatChange();
    }

    public void AfterChangeSpped()
    {
        try
        {
            if (gameObject != null && tag == "Enemy" && agent != null)
            {
                agent.speed = speedCurrent;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            PlayerStatChange();
        }

    }

    public void PlayerStatChange()
    {

        if (isPlayer)
        {
            PlayerInfo playerInfo = GameManagerPC.Instance.playerManagement.GetPlayerByID(unit.id);
            if (playerInfo != null && !playerInfo.isDisconnected)
            {
                MyServer.Instance.SendPlayerStatChangeMessage(
                    playerInfo,
                    unit.hpMax,
                    unit.hpCurrent,
                    damageManager.damageDataList[0].damage,
                    speedCurrent
                );
            }
        }
    }

}

public class AddExtraDamage : extraDamage
{

    public float damage;

    public AddExtraDamage(float damage)
    {
        this.damage = damage * 100;
    }

    public float getPercent()
    {
        return damage;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return ((AddExtraDamage)obj).damage == damage;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return (int)damage;
    }
}
