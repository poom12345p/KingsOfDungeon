using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class WizardControl : BaseCharacter, CharecterControl
{
    /// <summary>
    /// /Basic parametor
    /// </summary>
    [Header("Agent properties")]

    public float turnspeedReal, speedReal;
    public int spreadAngle;
    [Space]
    [HideInInspector] public unitHitbox hitbox;
    public NavMeshAgent agent;
    const string CHARGERELEASEcmd = "CHARGERELEASE", CHARGEUPcmd = "CHARGEUP", UNCASTcmd = "UNCAST";
    /// <summary>
    /// ////////////////////////////////////////////////////////
    /// </summary>
    [Header("Wizard invole")]
    public Transform wizardBody;
    public GuageRing manaRing, manaUltiRing;
    public Animator wizardAnimAvatar;
    [Header("Wizard skill")]
    public damageHitbox normalAtk, skill_1, skill_2, skill_3, skill_ulti;
    public damageHitbox normalAtk_base, skill_1_base, skill_2_base, skill_3_base, sub_1_base, skill_ulti_base;
    public Transform areaSpot, fireSpot;
    [Header("Wizard guideline")]
    public GameObject[] guideList;
    [Header("Wizard porperties")]
    public float mana;
    public float manaMax, chargespeed;
    private float manaUlti;
    public short chargeCount;
    public short chargeMax;
    [Header("skill porperties")]
    public int normalAtkNum;
    public float normalAtkAngle;
    [Space]
    public int cycloneNum;
    public float cycloneAngle;
    [Space]
    public int fireBallNum;
    public float fireBallAngle;
    [Space]
    public bool canSaveSkill;
    [Space]
    public bool isCast = false, isAtk = false;
    const int IDLE = -1, RUN = 2, WALK = 3, DEATH = 6, NORMALATK = 4, SKILL1 = 7, SKILL2 = 8, SKILL3 = 9, ULTI = 10;
    // Use this for initialization

    public AudioSource attackAudioSource;
    public AudioSource chargingAudioSource;
    public AudioSource chargeUpAudioSource;
    public AudioClip fireBallSound;
    public AudioClip tornadoSound;
    public AudioClip normalAttackSound;

    [Header("particle")]
    public ParticleSystem showAvatarParticle;
    public ParticleSystem hideAvatarParticle;
    public ParticleSystem chargeParticle;
    public ParticleSystem[] skillParticle;
    public Image[] ImageSkill = new Image[5];
    Rigidbody myRigibody;
    [Header("skill fire Area")]
    public damageHitbox fireArea;
    public bool canFireArea;

    void Start()
    {
        hitbox = GetComponent<unitHitbox>();
        statContol = GetComponent<StatContol>();
        speedReal = statContol.speedCurrent;
        manaRing.updateGaugeImediate(manaMax, 0);
        manaUltiRing.updateGaugeImediate(manaMax, 0);
        manaRing.gameObject.SetActive(false);
        wizardAnimAvatar.gameObject.SetActive(false);
        myRigibody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (id != -1)
        {
            x = CrossPlatformInputManager.VirtualAxisReference("Horizontal_" + id).GetValue;
            y = CrossPlatformInputManager.VirtualAxisReference("Vertical_" + id).GetValue;

            if (!isDeath)
            {

                if ((x != 0.00f || y != 0.00f) && !isAbnormalState())
                {
                    if (!isCast)
                    {
                        anim.SetInteger("Speed", RUN);
                    }
                    else
                    {
                        anim.SetInteger("Speed", WALK);
                        if (wizardAnimAvatar.gameObject.activeInHierarchy) wizardAnimAvatar.SetInteger("Speed", WALK);
                    }
                    transform.Translate(Vector3.forward * speedReal * Time.deltaTime);

                    angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 45;
                    //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);
                    if (angle < 0) angle += 360;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
                }
                else if (isAbnormalState())
                {

                }
                else
                {
                    anim.SetInteger("Speed", IDLE);
                    myRigibody.velocity = Vector3.zero;
                    if (wizardAnimAvatar.gameObject.activeInHierarchy) wizardAnimAvatar.SetInteger("Speed", IDLE);
                }


            }
        }
    }
    IEnumerator charging()
    {
        while (isCast)
        {
            yield return new WaitForFixedUpdate();
            if (chargeCount < chargeMax)
            {
                mana += chargespeed * Time.fixedDeltaTime;
                if (mana >= manaMax)
                {
                    guideList[chargeCount].SetActive(false);
                    if (skillParticle[chargeCount] != null) skillParticle[chargeCount].Stop();
                    if (ImageSkill[chargeCount] != null) ImageSkill[chargeCount].enabled = false;
                    //
                    chargeCount++;
                    GameManagerPC.Instance.sendMsgToController(id, "CHARGEUP");
                    //
                    skillParticle[chargeCount].Play();
                    if (ImageSkill[chargeCount] != null) ImageSkill[chargeCount].enabled = true;
                    guideList[chargeCount].SetActive(true);
                    mana = 0;
                    chargeUpAudioSource.Play();
                }
            }
            else if (ultimateManager.isFull())
            {
                mana = manaMax;
                if (manaUlti < manaMax)
                {
                    manaUlti += chargespeed * Time.fixedDeltaTime;
                    manaUltiRing.updateGaugeImediate(manaMax, manaUlti);
                    if (manaUlti >= manaMax)
                    {
                        manaUlti = manaMax;
                        chargeUpAudioSource.Play();
                        manaUltiRing.updateGauge(manaMax, manaUlti);
                        showAvatarParticle.Play();
                        guideList[chargeCount].SetActive(false);
                        skillParticle[chargeCount].Stop();
                        if (ImageSkill[chargeCount] != null) ImageSkill[chargeCount].enabled = false;
                        chargeCount = 4;
                        if (ImageSkill[chargeCount] != null) ImageSkill[chargeCount].enabled = true;
                        skillParticle[chargeCount].Play();
                        guideList[chargeCount].SetActive(true);
                        wizardAnimAvatar.gameObject.SetActive(true);
                    }
                }


            }
            else
            {
                mana = manaMax;

            }

            //if(mana >= manaMax)
            //{
            //    if (chargeCount < chargeMax)
            //    {
            //        guideList[chargeCount].SetActive(false);
            //        chargeCount++;
            //        guideList[chargeCount].SetActive(true);
            //        mana = 0;
            //    }
            //    else
            //    {
            //        mana = manaMax;
            //    }
            //}
            manaRing.updateGaugeImediate(manaMax, mana);
        }
    }
    public bool isAbnormalState()
    {
        return isAtk;
    }

    public void startCast()
    {
        if (!isAtk && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Skill_1"))
        {
            chargingAudioSource.Play();
            isCast = true;
            chargeParticle.Play();
            manaRing.gameObject.SetActive(true);
            anim.SetInteger("Speed", WALK);
            speedReal = statContol.speedCurrent * 0.4f;
            guideList[chargeCount].SetActive(true);
            StartCoroutine("charging");
        }
    }

    public void cancelCast()
    {
        if (isCast && !isAtk)
        {
            chargingAudioSource.Stop();
            isCast = false;
            isAtk =false;
            StopCoroutine("charging");
            chargeParticle.Stop();
            manaRing.gameObject.SetActive(false);
            wizardAnimAvatar.gameObject.SetActive(false);
            foreach (GameObject guide in guideList)
            {
                guide.SetActive(false);
            }
            foreach (ParticleSystem p in skillParticle)
            {
                if (p != null) p.Stop();
            }
            foreach (var Img in ImageSkill)
            {
                if (Img != null) Img.enabled = false;
            }
            anim.SetInteger("Speed", -1);
            if (!canSaveSkill)
            {
                chargeCount = 0;
                GameManagerPC.Instance.sendMsgToController(id, "RECHARGE");
            }
            mana = 0;
            speedReal = statContol.speedCurrent;
            manaUlti = 0;
            manaUltiRing.updateGauge(manaMax, manaUlti);
        }
    }

    public void stopCast()
    {
        if (!isAtk && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_1") && isCast)
        {
            chargingAudioSource.Stop();
            isCast = false;
            isAtk = true;
            StopCoroutine("charging");
            chargeParticle.Stop();
            manaRing.gameObject.SetActive(false);
            foreach (GameObject guide in guideList)
            {
                guide.SetActive(false);
            }
            foreach (ParticleSystem p in skillParticle)
            {
                if (p != null) p.Stop();
            }
            foreach (var Img in ImageSkill)
            {
                if (Img != null) Img.enabled = false;
            }
            switch (chargeCount)
            {
                case 0:
                    setAnimAction(NORMALATK);
                    break;
                case 1:
                    setAnimAction(SKILL1);
                    break;
                case 2:
                    setAnimAction(SKILL2);
                    break;
                case 3:
                    setAnimAction(SKILL3);
                    break;
                case 4:
                    setAnimAction(ULTI);
                    showAvatarParticle.Stop();
                    hideAvatarParticle.Play();
                    GameManagerPC.Instance.sendMsgToController(id, "ULTIEND");
                    if (wizardAnimAvatar.gameObject.activeInHierarchy) wizardAnimAvatar.SetInteger("Speed", ULTI);
                    break;

            }
            mana = 0;
            speedReal = statContol.speedCurrent;
            manaUlti = 0;
            manaUltiRing.updateGauge(manaMax, manaUlti);
        }

    }
    public void startAtk()
    {
        isAtk = true;
    }
    public void stopAtk()
    {
        isAtk = false;
    }
    public void fireMagic()
    {
        damageHitbox dambox;
        switch (chargeCount)
        {
            case 0:

                for (int i = 0; i < normalAtkNum; i++)
                {
                    dambox = spawnMagic(normalAtk_base, normalAtk, fireSpot);
                    dambox.transform.eulerAngles += new Vector3(0, normalAtkAngle * (Mathf.Pow(-1, i)) * (Mathf.Floor(((float)i + 1) / 2)), 0);
                }
                break;
            case 1:
                attackAudioSource.PlayOneShot(fireBallSound);
                for (int i = 0; i < fireBallNum; i++)
                {
                    dambox = spawnMagic(skill_1_base, skill_1, fireSpot);
                    dambox.transform.eulerAngles += new Vector3(0, fireBallAngle * (Mathf.Pow(-1, i)) * (Mathf.Floor(((float)i + 1) / 2)), 0);
                    ChainAttack CATK = dambox.gameObject.GetComponent<ChainAttack>();
                    CATK.chainList[0].model = sub_1_base;
                    if (canFireArea) CATK.chainList[1].model = fireArea;
                }
                break;
            case 2:
                attackAudioSource.PlayOneShot(tornadoSound);
                for (int i = 0; i < cycloneNum; i++)
                {
                    dambox = spawnMagic(skill_2_base, skill_2, fireSpot);
                    dambox.transform.eulerAngles += new Vector3(0, cycloneAngle * (Mathf.Pow(-1, i)) * (Mathf.Floor(((float)i + 1) / 2)), 0);
                }
                //spawnMagic(skill_2_base, skill_2, fireSpot);
                break;
            case 3:
                //spawnMagic(skill_3_base, skill_3, areaSpot);
                break;
            case 4:
                ultimateManager.resetUltimate();
                chargeCount = 0;
                manaUlti = 0;
                manaUltiRing.updateGauge(manaMax, manaUlti);
                spawnMagic(skill_ulti_base, skill_ulti, areaSpot);
                break;
        }
        chargeCount = 0;
        GameManagerPC.Instance.sendMsgToController(id, "RECHARGE");
        mana = 0;

    }

    public void endAct()
    {
        isAtk = false;
        wizardBody.localRotation = Quaternion.identity;
        if (wizardAnimAvatar.gameObject.activeInHierarchy) wizardAnimAvatar.gameObject.SetActive(false);
    }
    public void setAnimAction(int i)
    {
        anim.SetInteger("Speed", i);
    }
    public damageHitbox spawnMagic(damageHitbox myModel, damageHitbox myPrefab, Transform spot)
    {

        damageHitbox dambox = Instantiate(myPrefab);
        Bullet myBulltet = dambox.GetComponent<Bullet>();
        Bullet modelBullet = myModel.GetComponent<Bullet>();
        dambox.transform.position = spot.position;
        dambox.transform.eulerAngles = myModel.transform.eulerAngles;
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(myModel.damage);
        dambox.duplicateAfterDoneDamage(myModel);
        if (myBulltet != null)
        {
            myBulltet.setSpeed(modelBullet.speed);
            myBulltet.setBulletDistance(modelBullet.bulletDistance);
        }
        dambox.effect = myModel.effect;
        unitHitbox hitbox = gameObject.GetComponent<unitHitbox>();
        dambox.setOwner(hitbox);
        dambox.ultiRegen = myModel.ultiRegen;
        return dambox;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////controlcharecter/////////////////////////////////////////////////////////////////
    /// </summary>
    override public void getCommand(string cmd)
    {
        if (isDeath) return;
        // Debug.Log(cmd);
        switch (cmd)
        {
            case CHARGEUPcmd:
                startCast();
                break;

            case CHARGERELEASEcmd:
                stopCast();
                break;
            case UNCASTcmd:
                cancelCast();
                break;
            default:
                string[] cmds = cmd.Split('-');
                //Debug.Log(cmds[1]);
                switch (cmds[0])
                {
                    case "CASTANGLE":
                        if (isCast)
                        {
                            wizardBody.eulerAngles = new Vector3(0, -float.Parse(cmds[1]) - 90, 0);
                        }
                        break;
                }
                base.getCommand(cmd);
                break;

        }
    }

    override public void getUpSkill(int num)
    {
        switch (num)
        {
            case 1:
                chargespeed += 1;
                break;
            case 2:
                canSaveSkill = true;
                break;
            case 3:
                skill_2_base.GetComponent<Bullet>().bulletDistance += 2;
                break;
            case 4:
                normalAtk_base.GetComponent<Bullet>().bulletDistance += 2;
                break;
            case 5:
                normalAtkNum += 2;
                break;
            case 6:
                cycloneNum += 2;
                break;
            case 7:
                fireBallNum += 2;
                break;
            case 8:
                canFireArea = true;
                break;

            case 101:
                hitbox.hpMax += 300;
                hitbox.takenHeal(300);
                break;
            case 102:
                statContol.ChangeSpeedPercent(0.2f);
                break;
            case 103:
                hitbox.damageManager.plusExtraDamage(0.2f);
                break;
        }
        Debug.Log("wizard up skill|" + num);

    }

    public void setID(int _id)
    {
        id = _id;
        //Debug.Log("Archer get id: " + id);
    }
    public void recivceDamage(int damage)
    {

    }
    public void recivceHeal(int heal)
    {

    }
    public void ReportDeath()
    {
        resetAnim();
        isDeath = true;
        mana = 0;
        anim.SetInteger("Speed", 6);
    }
    public void ReportRevive()
    {
        resetAnim();
        isDeath = false;
        anim.SetInteger("Speed", -1);
    }

    public void TestSkillAttackWhileNotShow(int skill)
    {
        chargeCount = (short)skill;
        fireMagic();
    }

    void resetAnim()
    {
        cancelCast();
        endAct();
        stopAtk();
        mana = 0;
    }

}


