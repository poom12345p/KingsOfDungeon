using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class KnightControl : BaseCharacter, CharecterControl, afterTakenDamage, extraDamage
{


    [Header("charge modify")]
    public float chargeWaitTime, chargeStayTime;
    [Space]

    public int baseStamina;
    int atkDelay;

    public bool isAtk, isReceiving, isBlocking, isCharged, isPrefromUlti;
    int nextAct = 0;
    [SerializeField] int chargState = 0;
    [Header("extra scripts")]
    public GameObject normalAtk, shieldEff;
    public unitHitbox hitbox;
    public GuageRing staminaGuage;

    public KnightUltiControl KnightUlti;
    Rigidbody myRigibody;
    [Header("skill modify")]
    public damageHitbox shackwaveDB;
    public bool canShockwave, canReflect;
    public float atkSpeed = 1;
    public float walkSpeed = 0.2f;
    [Header("damageHitbox")]
    public damageHitbox normalAtkDH, swingChargeDH, blockAtkDH, ultiSDH, ultiLDH, jumpDH, shockwaveDH;
    [SerializeField]
    [Header("stamina")]
    public float stamina;
    float maxStamina;
     float baseStaRegen = 10;
    const float baseStaBlockRegen = 1, baseStaBlockUse = 5, baseChargeUse = 3; int touchIndexBow, touchIndexAnalog;
    [SerializeField] private chargeState state = chargeState.NONE;
    public enum damageHitboxTag
    {
        NORMAL,
        SWING,
        SHILD,
        ULTI_S,
        ULTI_L,
        JUMPING
    }

    public enum chargeState
    {
        CHARED,
        CHARGING,
        UNCHARGING,
        NONE
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //SOUND

    public AudioClip swordSlashSound;

    public AudioClip WalkSound;
    int checkSound;

    public ParticleSystem runChargeParticle, ultiReadyParticle, showAvatarParticle, hideAvatarParticle, swordParticle, jumpParticle;
    // Use this for initialization



    void Start()
    {
        myRigibody = GetComponent<Rigidbody>();
        statContol = GetComponent<StatContol>();
        stamina = maxStamina = baseStamina;
        updateStamina();
        hitbox = gameObject.GetComponent<unitHitbox>();
        hitbox.addAfterTakenDamage(this);
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        isReceiving = true;
        // normalAtkDH.deActiveHitbox();
        //StartCoroutine(regenStamina());
    }
    //IEnumerator regenStamina()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(0.25f);
    //        if (stamina < maxStamina)
    //        {
    //            if (isBlocking)
    //            {
    //                stamina += baseStaBlockRegen* 0.25f;
    //            }
    //            else
    //            {
    //                stamina += baseStaRegen;
    //            }
    //            updateStamina();
    //        }
    //        if (stamina <= 0)
    //        {
    //            performUnBlock();
    //            stamina = 0;
    //        }
    //        if (stamina >= maxStamina)
    //        {
    //            stamina = maxStamina;
    //        }
    //    }
    //}

    public IEnumerator waitForCharge()
    {
        if (!isAtk && state == chargeState.CHARGING)
        {
            yield return new WaitForSeconds(chargeWaitTime);
            //isCharging = true;
            if (state == chargeState.CHARGING) startCharge();
        }

    }
    IEnumerator chargeDelay()
    {
        // state = chargeState.UNCHARGING;
        if (isCharged && state == chargeState.CHARED)
        {
            //Debug.Log("flase Charging");
            yield return new WaitForSeconds(chargeStayTime);
            chargeRelease();
        }

    }

    void FixedUpdate()
    {
        if (id != -1)
        {
            x = CrossPlatformInputManager.VirtualAxisReference("Horizontal_" + id).GetValue;
            y = CrossPlatformInputManager.VirtualAxisReference("Vertical_" + id).GetValue;
            if (!isDeath)
            {
                if (!isAtk && !isPrefromUlti)
                {
                    if ((x != 0.00f || y != 0.00f) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
                    {

                       if(!isBlocking)
                            transform.Translate(Vector3.forward * statContol.speedCurrent * Time.deltaTime);
                       else
                            transform.Translate(Vector3.forward * statContol.speedCurrent * Time.deltaTime* walkSpeed);
                        //anim.SetInteger("Speed", 2);
                        anim.SetBool("isRunning", true);


                    }
                    else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
                    {
                        //anim.SetInteger("Speed", -1);
                        anim.SetBool("isRunning", false);
                        myRigibody.velocity = Vector3.zero;
                        /* if (isCharging && isCharged)
                         {
                             StartCoroutine(chargeDelay());
                         }*/
                    }
                }

                if ((x != 0.00f || y != 0.00f) && !isPrefromUlti)
                {
                    angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 45;
                    //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);

                    if (angle < 0) angle += 360;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
                }
                if (stamina < maxStamina && chargState == 0)
                {
                    if (isBlocking)
                    {
                        stamina += baseStaBlockRegen * Time.deltaTime;
                    }
                    else
                    {
                        stamina += baseStaRegen * Time.deltaTime;
                    }
                    updateStamina();
                }
                /* else if (chargState > 0)
                 {
                     stamina -= chargState * Time.deltaTime;
                     updateStamina();
                 }*/
                if (stamina <= 0)
                {
                    performUnBlock();
                    stamina = 0;
                }
                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }


        }

    }

    override public void getCommand(string cmd)
    {
        if (isDeath) return;
        switch (cmd)
        {
            case "ATTACK":
                performAttack();
                break;
            case "BLOCK":
                StopCoroutine("chargeDelay");
                if (state == chargeState.CHARGING && !isCharged)
                {
                    StopCoroutine("waitForCharge");
                    state = chargeState.NONE;
                }
                StartCoroutine("chargeDelay");
                performBlock();
                // Attack();
                break;
            case "UNBLOCK":
                performUnBlock();
                break;
            case "CHARGEUP":
                if (!isCharged)
                {
                    chargeUP();
                }

                break;
            case "CHARGERELEASE":
                StopCoroutine("chargeDelay");
                if (state == chargeState.CHARGING && !isCharged)
                {
                    StopCoroutine("waitForCharge");
                    state = chargeState.NONE;
                }
                StartCoroutine("chargeDelay");
                break;
            case "ULTIMATE":

                    preformUltimate();
                
                break;
            default:
                base.getCommand(cmd);

                break;
        }
    }
     override public void getUpSkill(int num)
    {
        switch (num)
        {
            case 1:
                canShockwave = true;
                break;
            case 2:
                maxStamina += 50;
                break;
            case 3:
                baseStaRegen += 5;
                break;
            case 4:
                canReflect = true;
                break;
            case 5:
               Bullet  sb= shockwaveDH.GetComponent<Bullet>();
                sb.bulletDistance += 4;
                break;
            case 6:
                walkSpeed += 0.3f;
                break;
            case 7:
                atkSpeed += 1.5f;
                break;
            case 8:

                break;
            default:
                base.getUpSkill(num);
                break;
                //////////////////////////////////////////////////////////////////
            //case 101:
            //    hitbox.hpMax += 300;
            //    hitbox.takenHeal(300);
            //    break;
            //case 102:
            //    statContol.ChangeSpeedPercent(0.2f);
            //    break;
            //case 103:
            //    hitbox.damageManager.plusExtraDamage(0.2f);
            //    break;
                //case 0:
                //    canShockwave = true;

                //    break;
                //case 1:
                //    maxStamina += 50;
                //    break;
                //case 2:
                //    baseStaRegen += 5;
                //    break;
                //case 3:
                //    if (chargeWaitTime < 1f) chargeWaitTime -= 0.5f;
                //    break;
                //case 4:
                //    canReflect = true;
                //    break;
                //case 5:
                //    shockwaveDH.GetComponent<Bullet>().bulletDistance += 2;
                //    break;
                //case 101:
                //    hitbox.hpMax += 200;
                //    hitbox.takenHeal(200);
                //    break;
                //case 102:
                //    statContol.ChangeSpeedNormal(2);
                //    break;
                //case 103:
                //    hitbox.damageManager.addExtraDamage(this);
                //    break;
        }

    }
    public void setID(int _id)
    {
        id = _id;
        //Debug.Log("Knight get id: " + id);
    }
    public void performAttack()
    {
        //Debug.Log("preformATk");
        //if (isCharging) return;
        swordParticle.Play();
        if (state == chargeState.CHARGING && !isCharged)
        {
            StopCoroutine("waitForCharge");
            state = chargeState.NONE;
        }

        StopCoroutine("chargeDelay");
        //-----------------------------------------------
        if (!isCharged)
        {

            isAtk = true;
            //Debug.Log("isCharge preform 1");
            //if (anim.GetInteger("Speed") < 3)
            //{

            //    anim.SetInteger("Speed", 3);
            //}
            anim.SetTrigger("ATK");
            if (isBlocking)
            {
                //anim.SetInteger("Speed", 0);
                hitbox.isImmortal = false;
                updateStamina();
                shieldEff.SetActive(false);
            }
            anim.speed = atkSpeed;
        }
        else
        {

            //Debug.Log("isCharge preform 3");   
            performAtk3C();
        }

        anim.SetFloat("Accel", 1f);
        //----------------------------------------------
        //if (!isAtk && !isBlocking)
        //{

        //    isReceiving = false;
        //    if (!isCharged)
        //    {

        //        isAtk = true;
        //        //Debug.Log("isCharge preform 1");
        //        //if (anim.GetInteger("Speed") < 3)
        //        //{

        //        //    anim.SetInteger("Speed", 3);
        //        //}
        //        anim.SetTrigger("ATK");
        //    }
        //    else
        //    {

        //        //Debug.Log("isCharge preform 3");   
        //        performAtk3C();
        //    }
        //    anim.SetFloat("Accel", 1f);

        //    //setNormalAtk();
        //}
        //else if(isBlocking)
        //{
        //    anim.SetTrigger("ATK");
        //    //anim.SetInteger("Speed", 0);
        //    hitbox.isImmortal = false;
        //    updateStamina();
        //    shieldEff.SetActive(false);
        //}
        //----------------------------

    }

    void performAtk3C()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Combo01") && state == chargeState.CHARED)
        {
            chargeRelease();
            isAtk = true;
            //Debug.Log("preformATk3");
            StopCoroutine("chargeDelay");
            isAtk = true;
            //  isCharging = false;
            anim.SetInteger("Speed", 11);
            anim.SetFloat("Accel", 1f);
        }
    }
    void performEndAtk()
    {
        isAtk = false;
        swordParticle.Stop();
        anim.speed = 1;
        anim.SetInteger("Speed", 0);
        if (isBlocking)
        {
            hitbox.isImmortal = true;
            updateStamina();
            shieldEff.SetActive(true);
            anim.ResetTrigger("ATK");
        }

    }

    public void createShockwave()
    {
        if (canShockwave) shackwaveDB.spawnDuplicate(shackwaveDB.transform);
    }
    //void setNormalAtk()
    //{
    //    Invoke("ActiveNormalAtkHitbox", 0.4f);
    //    Invoke("deActiveNormalAtkHitbox", 0.75f);
    //    Invoke("delayReReceiving", 0.75f);
    //    Invoke("delayAtkCount", 1.0f);
    //}
    //public void deActiveNormalAtkHitbox()
    //{
    //    normalAtk.GetComponent<damageHitbox>().deActiveHitbox();
    //}
    //public void ActiveNormalAtkHitbox()
    //{
    //    normalAtkDH.activeHitbox();
    //}

    public void performBlock()
    {
        if (stamina > 0)
        {
            isBlocking = true;
            // anim.SetInteger("Speed", 5);
            anim.SetBool("isGuard", true);
            hitbox.isImmortal = true;
            updateStamina();
            shieldEff.SetActive(true);
        }
    }
    public void performUnBlock()
    {
        isBlocking = false;
        //anim.SetInteger("Speed", 0);
        anim.SetBool("isGuard", false);
        hitbox.isImmortal = false;
        updateStamina();
        shieldEff.SetActive(false);
    }

    void updateStamina()
    {
        if (isBlocking)
        {
            staminaGuage.gaugeImage.enabled = true;

        }
        else if (stamina >= maxStamina)
        {
            staminaGuage.gaugeImage.enabled = false;
            stamina = maxStamina;
        }
        else if (stamina <= maxStamina)
        {
            staminaGuage.gaugeImage.enabled = true;
        }

        staminaGuage.updateGauge(maxStamina, stamina);

    }
    public void recivceDamage(int damage)
    {
        if (isBlocking && shieldEff.active)
        {
            //Debug.Log("stamina decreas");
            stamina -= baseStaBlockUse;
            updateStamina();
        }
    }
    public void chargeUP()
    {

        //Debug.Log("startCharge");
        if (!isCharged && state == chargeState.NONE)
        {
            state = chargeState.CHARGING;
            StartCoroutine("waitForCharge");
        }
        else if (isCharged && state == chargeState.CHARED)
        {
            StopCoroutine("chargeDelay");
        }
        /*  if (chargState < 3)
          {
              chargState++;
              isCharging = true;
          }*/

    }
    public void chargeRelease()
    {
        if (state == chargeState.CHARED && isCharged)
        {
            //Debug.Log("stopCharge");
            state = chargeState.NONE;
            //StopCoroutine("waitForCharge");
            //StartCoroutine("chargeDelay");
            runChargeParticle.Stop();
            isCharged = false;
            statContol.ChangeSpeedPercent(-0.2f);
        }
        // chargState = 0;
    }


    public void hideHitbox(damageHitboxTag damageTag)
    {
        fitterDH(damageTag).deActiveHitbox();
    }

    public void activeHitbox(damageHitboxTag damageTag)
    {
        fitterDH(damageTag).activeHitbox();
    }

    public void hideGameObj(damageHitboxTag damageTag)
    {
        fitterDH(damageTag).gameObject.SetActive(false);
    }

    public void activeGameObj(damageHitboxTag damageTag)
    {
        fitterDH(damageTag).gameObject.SetActive(true);
    }
    damageHitbox fitterDH(damageHitboxTag atkTag)
    {
        damageHitbox myDH;
        switch (atkTag)
        {
            case damageHitboxTag.NORMAL:
                myDH = normalAtkDH;
                break;
            case damageHitboxTag.SWING:
                myDH = swingChargeDH;
                break;
            case damageHitboxTag.SHILD:
                myDH = blockAtkDH;
                break;
            case damageHitboxTag.ULTI_L:
                myDH = ultiLDH;
                break;
            case damageHitboxTag.ULTI_S:
                myDH = ultiSDH;
                break;
            case damageHitboxTag.JUMPING:
                myDH = jumpDH;
                break;
            default:
                return null;

        }
        return myDH;
    }

    damageHitbox fitterDH(string atkTag)
    {
        damageHitbox myDH;
        switch (atkTag)
        {
            case "NORMAL":
                myDH = normalAtkDH;
                break;
            case "SWING":
                myDH = swingChargeDH;
                break;
            case "SHIELD":
                myDH = blockAtkDH;
                break;

            default:
                return null;

        }
        return myDH;
    }
    public void startCharge()
    {
        //Debug.Log("chargeUp");
        state = chargeState.CHARED;
        runChargeParticle.Play();
        statContol.ChangeSpeedPercent(0.2f);
        isCharged = true;
    }

    public void preformUltimate()
    {
        if (ultimateManager.isFull() && !isBlocking)
        {
            isPrefromUlti = true;
            ultimateManager.useUltimate();
            showAvatarParticle.Play();
            KnightUlti.gameObject.SetActive(true);
            KnightUlti.activeUltimate();
        }
        else if (isPrefromUlti)
        {
            KnightUlti.TriggerUltimate();

            //Debug.Log("ora");
        }
        else
        {
            isPrefromUlti = false;
            isAtk = false;
        }
    }

    public void endUltimate()
    {
        isPrefromUlti = false;
        isAtk = false;
        KnightUlti.gameObject.SetActive(false);
        hideAvatarParticle.Play();
        ultimateManager.endUltimate();
    }

    public void clearAllATK()
    {
        performEndAtk();
        hideHitbox(damageHitboxTag.NORMAL);
        hideHitbox(damageHitboxTag.SWING);
    }

    public void playJumpParticle()
    {
        jumpParticle.Play();
    }


    public void resetAll()
    {
        isAtk = isReceiving= isBlocking= isCharged= isPrefromUlti=false;
        KnightUlti.EndUltimate();
        endUltimate();
        performUnBlock();
        swordParticle.Stop();

    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public void ReportDeath()
    {
        resetAll();
        isDeath = true;
        isAtk = false;
        anim.SetInteger("Speed", 6);
       
        source.PlayOneShot(deadSound);
        //if (GameManagerPC.Instance != null)
        //{
        //    GameManagerPC.Instance.PlayerDeath(id);
        //}
    }

    public void ReportRevive()
    {
        isDeath = false;
        anim.SetInteger("Speed", 0);
    }

    public void recivceHeal(int heal)
    {

    }

    


    public void doActionATD(int damageTaken, unitHitbox takenFrom)
    {
        if (canReflect && isBlocking)
        {
            int rfDam = (int)((float)damageTaken * 0.3f);
            if (rfDam == 0) rfDam = 1;
            takenFrom.takenDamage(rfDam, hitbox);
        }
    }
    ///////////////////////////////
    public float getPercent()
    {
        return 0.2f;
    }



}
