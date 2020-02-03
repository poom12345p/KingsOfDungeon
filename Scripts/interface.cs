using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface CharecterControl
{
    void getCommand(string cmd);
    void getUpSkill(int num);
    void setID(int id);
    void recivceDamage(int damage);
    void recivceHeal(int heal);

    // death
    void ReportDeath();
    void ReportRevive();
    void ReportUltiFull();
    void ReportToClient(string msg);
    UltimateManager GetUltiManager();
    StatContol getCharacterStat();
}
interface EnemyControl
{
    // death
    void ReportDeath();
    StatContol getCharacterStat();

}

interface Potion
{

    void getPotion(unitHitbox player);
    particleControl getParticle();
}

public interface afterHeal
{
    void doActionAH();
}

public interface afterTakenDamage
{

    void doActionATD(int damageTaken, unitHitbox takenFrom);
}

public interface afterDoneDamage
{
    void doActionADD(unitHitbox attacker, unitHitbox target, damageHitbox damageHB);
}

public interface afterPickItem
{
    void doActionAPI(unitHitbox unitHitbox);
    bool checkPickItemCondition(unitHitbox picker);
}

public interface extraDamage
{
    float getPercent();
}

public interface extraUltiRegen
{
    float getPercent();
}

public interface additionalSpeed
{
    float getPercent();
}
public interface afterDeath
{
    void doAction();
}

public interface beforeDestory
{
    void doAction();
}
public interface Controller
{
    void receiveMsg(string msg);
}

public interface itemEffect
{
    // set the owner of this item effect
    void SetOwnerInstance(unitHitbox unit);
    // what to do when get more of this item
    void GetMoreItem();
    // Remove Effect of item on player and destroy self
    void RemoveEffect();
}

public interface MsgReciver
{
    void reciveMsg(string msg);
}
public enum InteractType
{
    NONE,SHAKE,UP,LEFT,RIGHT,DOWN
}