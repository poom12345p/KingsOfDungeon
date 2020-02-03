using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class LancerControl : BaseCharacter, CharecterControl
{
    [Header("basic")]
    public damageHitbox warpATK;
    public Animator lanceAnim;
    public LancerAnimControl lac;
    public Transform mainBody;
    public SpearRush spearRush;
    public GameObject ultiAvatar;
    public bool canRush = false, isCharged = false, isRdyWarp = false;
    public float chargeTime, RushspeedUpfactor;
    [Header("particle")]
    public ParticleSystem rdyWarpParticle;
    [Header("Ulti modify")]
    public bool canUlti = false;
    public bool canUltiSppedUp = true;
    public float untiChargeTime, ultiDuration;
    public damageHitbox fireArea;
    public float fireAreaFreq;
    public Transform ultiPointer;
    public ParticleSystem ultiFeedback;
    [Header("skill modify")]
    [SerializeField] private float ultiSpeedFactor = 0f;
    const int IDLE = -1, RUN = 2;
    const string ATKDOWNcmd = "ATKDOWN", ATKUPcmd = "ATKUP",
         ANALOGDOWN = "ANALOGDOWN", ANALOGUP = "ANALOGUP", ULTISHAKE = "ULTISHAKE";
    public enum State
    {
        IDLE, RUN, ATK, RUSH, DEATH, NONE, ULTI
    }
    [System.Serializable]
    struct virsualColider
    {
        public Vector3 size;
        public Vector3 center;
    }
    [SerializeField] virsualColider[] extraAtkCol;
    public ParticleSystem[] extraAtkparticle;
    public BoxCollider normalAtkCol;
    public int atkLongFacter = 0;
    public bool isFireAfterWarp;
    public SpearRush SRPinBox;
    public int warpLoop = 0;
    public int doubleAtkchance = 0;
    public float rushTrunSpeedMutiply = 0.01f;
    [SerializeField] State myState = State.IDLE;
    // Use this for initialization
    void Start()
    {
        ultiPointer.gameObject.SetActive(false);
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


                if (myState != State.RUSH)
                {
                    if (x != 0.00f || y != 0.00f)
                    {



                        if (myState != State.ATK) transform.Translate(mainBody.forward * statContol.speedCurrent * Time.deltaTime);
                      //  anim.SetInteger("Speed", 2);
                        angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 45;
                        //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);
                        if (angle < 0) angle += 360;
                        mainBody.rotation = Quaternion.Lerp(mainBody.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
                    }
                    else
                    {
                        anim.SetInteger("Speed", -1);
                    }
                }
                else
                {
                    angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 45;
                    if (ultimateManager.isUltimating)
                    {

                        ultiPointer.rotation = Quaternion.Lerp(ultiPointer.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
                        //mainBody.rotation = Quaternion.Lerp(mainBody.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed * 0.01f);
                        //transform.Translate(mainBody.forward * statContol.speedCurrent * Time.deltaTime * (ultiSpeedFactor + 0.2f));
                    }
                    else
                    {
                        mainBody.rotation = Quaternion.Lerp(mainBody.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed * rushTrunSpeedMutiply);


                    }
                    transform.Translate(mainBody.forward * statContol.speedCurrent * Time.deltaTime * (1f + RushspeedUpfactor));
                    // else
                    // {


                    // }

                }
            }


        }
    }

    IEnumerator Charging()
    {
        yield return new WaitForSeconds(chargeTime);
        /*  isCharged = true;
          if (ultimateManager.isFull()) canUlti = true;*/
        isRdyWarp = true;
        rdyWarpParticle.Play();


    }

    IEnumerator UltiDuration()
    {
        float timeCount = 0;
        // const float triggerTime = 0.5f;
        float triggerTime = fireAreaFreq;
        while (timeCount < ultiDuration)
        {
            //yield return new WaitForSeconds(triggerTime);
            //timeCount += triggerTime;
            //if(ultiSpeedFactor> 0 &&canUltiSppedUp)
            //{
            //    ultiSpeedFactor -= 0.25f;
            //}
            yield return new WaitForSeconds(triggerTime);
            timeCount += triggerTime;
            fireArea.spawnDuplicate(fireArea.transform);

        }

        ultimateManager.endUltimate();
        setUnRushUlti();
        ReportToClient("ULTIEND");
        setRun();
        //end prefrom ulti

    }

    void setRun()
    {
        //  anim.SetInteger("Speed", 2);
        lac.setAnimRun();
        myState = State.RUN;
    }
    void setUnRun()
    {
        // anim.SetInteger("Speed", -1);
        lac.setAnimUnRun();
        myState = State.IDLE;


    }

    void setRush()
    {
        myState = State.RUSH;

        lac.PlayRushPaticle();
        // statContol.ChangeSpeedPercent(RushspeedUpfactor);
        lac.setAnimRush();
        spearRush.gameObject.SetActive(true);
        spearRush.reset();
        lac.ActiveDambox(LancerAnimControl.DamboxTag.rushAtk);
        canRush = false;
    }

    void setUnRush()
    {
        myState = State.NONE;
        lac.StopRushPaticle();
        lac.UnActiveDamBox(LancerAnimControl.DamboxTag.rushAtk);
        // statContol.ChangeSpeedPercent(-RushspeedUpfactor);
        spearRush.reset();
        spearRush.gameObject.SetActive(false);

        lac.setAnimUnRush();
    }
    void setUnRushUlti()
    {
        myState = State.NONE;
        lac.StopRushPaticle();
        statContol.ChangeSpeedPercent(-RushspeedUpfactor);
        spearRush.reset();
        spearRush.gameObject.SetActive(false);
        ultiPointer.gameObject.SetActive(false);
        lac.UnActiveDamBox(LancerAnimControl.DamboxTag.Ulti);
        lac.setAnimUnRush();
        ultiSpeedFactor = 0;
        spearRush.maxtarget -= 6;

        //hide avatar
        ultiAvatar.SetActive(false);
    }

    public void setUlti()
    {
        myState = State.RUSH;

        lac.PlayRushPaticle();
        statContol.ChangeSpeedPercent(RushspeedUpfactor);
        spearRush.gameObject.SetActive(true);
        lac.setAnimRush();
        //

        ultiPointer.gameObject.SetActive(true);
        ultiPointer.rotation = mainBody.rotation;
        //
        spearRush.reset();
        lac.ActiveDambox(LancerAnimControl.DamboxTag.Ulti);
        ultimateManager.useUltimate();
        ultiAvatar.SetActive(true);
        canRush = false;
        canUlti = false;
        spearRush.maxtarget += 6;
        StartCoroutine("UltiDuration");
    }
    void setWarp()
    {
        lac.SetAnimWarp();

        warpATK.spawnDuplicate(warpATK.transform);
        if (isFireAfterWarp)
        {
            fireArea.spawnDuplicate(fireArea.transform);
        }
        transform.Translate(mainBody.forward * 3.5f);

        for (int i = 0; i < warpLoop; i++)
        {
            warpATK.spawnDuplicate(warpATK.transform);
            transform.Translate(mainBody.forward * 3.5f);
        }


    }
    void DelayRush()
    {
        if (myState != State.RUSH)
        {
            //myState = State.RUSH;
            canRush = false;
            canUlti = false;
            //setAnimRush();
        }
    }

    public void AtkBtndown()
    {
        if (myState != State.RUSH)
        {
            canRush = false;
            StartCoroutine("Charging");
        }
    }
    public void AttackRelease()
    {
        if (myState != State.RUSH)
        {
            canRush = false;
            if (!isRdyWarp)
            {
                lac.setAnimAtk();
                StopCoroutine("Charging");
            }
            else
            {
                //lac.setAnimwarp
                //warp
                setWarp();
                isRdyWarp = false;
                rdyWarpParticle.Stop();
            }
            myState = State.ATK;
        }
    }

    public void endATKAct()
    {
        myState = State.NONE;
    }

    public void UltiTrigger()
    {
        if (ultimateManager.isUltimating)
        {
            //if (ultiSpeedFactor < 1f && canUltiSppedUp)
            //{
            //    ultiSpeedFactor += 0.25f;
            //    canUltiSppedUp = false;
            //    ultiFeedback.Play();
            //    Invoke("DelaySpeedUp", 0.1f);
            //}
            mainBody.rotation = Quaternion.Lerp(ultiPointer.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed * 0.01f);
        }
        else if (ultimateManager.isFull())
        {

            setUlti();

        }

    }
    public void DelaySpeedUp()
    {
        canUltiSppedUp = true;
    }
    public void DelayCanRush()
    {
        canRush = false;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////controlcharecter/////////////////////////////////////////////////////////////////
    /// </summary>
    override public void getCommand(string cmd)
    {
        if (isDeath) return;
        Debug.Log(cmd);
        switch (cmd)
        {

            case ATKDOWNcmd:
                AtkBtndown();
                break;
            case ATKUPcmd:
                AttackRelease();
                break;
            case ANALOGUP:
                getAnalogUp();
                // hideAvatar();
                break;
            case ANALOGDOWN:
                getAnalogDown();
                // hideAvatar();
                break;
            case ULTISHAKE:
                UltiTrigger();
                // hideAvatar();
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
                atkLongFacter++;
                normalAtkCol.size = extraAtkCol[atkLongFacter - 1].size;
                normalAtkCol.center = extraAtkCol[atkLongFacter - 1].center;
                break;
            case 2:
                doubleAtkchance+=25;
                break;
            case 3:
                isFireAfterWarp = true;
                break;
            case 4:
                rushTrunSpeedMutiply += 0.05f;
                break;
            case 5:
                SRPinBox.maxtarget++;
                break;
            case 6:
                warpLoop++;
                break;
            default:
                base.getUpSkill(num);
                break;
        }

    }

    public void getAnalogDown()
    {
        if (!canRush && myState != State.RUSH)
        {
            setRun();
            // StartCoroutine("Charging");
        }

        else if (canRush && !ultimateManager.isUltimating)
        {
            if (myState != State.RUSH)
            {
                setRush();
  
            }
            else
            {
                setUnRush();
            }

        }
        canRush = true;
        Invoke("DelayCanRush", 0.2f);


    }
    public void getAnalogUp()
    {
        //if (myState != State.RUSH)
        //    setUnRun();
        if (myState == State.RUSH)
        {
            setUnRush();
            //canRush = true;

            //if (ultimateManager.isUltimating)
            //{
            //    StopCoroutine("UltiDuration");
            //    ultimateManager.endUltimate();
            //    setUnRushUlti();
            //}
        }
        else
        {
            setUnRun();

        }



        if (ultimateManager.isFull()) canUlti = true;
        //if (isCharged)
        //{
        //    canRush = true;
        //}
        //else
        //{
        //    StopCoroutine("Charging");
        //}
        // Invoke("DelayRush", 0.2f);

        //isCharged = false;

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
        resetanim();
        isDeath = true;

        lac.SetAnimDeath();
        source.PlayOneShot(deadSound);


    }

    void resetanim()
    {
        ReportToClient("ULTIEND");
        lac.setAnimEndAtk();
        endATKAct();
        setUnRun();
        setUnRush();
        if (ultimateManager.isUltimating)
        {
            setUnRushUlti();
        }
        else
        {
            myState = State.NONE;
            lac.StopRushPaticle();
            spearRush.reset();
            spearRush.gameObject.SetActive(false);
            ultiPointer.gameObject.SetActive(false);
            lac.UnActiveDamBox(LancerAnimControl.DamboxTag.Ulti);
            lac.setAnimUnRush();
            ultiSpeedFactor = 0;
        }
    }
    public void ReportRevive()
    {
        ReportToClient("ULTIEND");
        isDeath = false;
        lac.RestAnim();
        anim.SetInteger("Speed", -1);
    }


}
