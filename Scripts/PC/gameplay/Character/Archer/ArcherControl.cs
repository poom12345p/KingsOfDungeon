using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class ArcherControl : BaseCharacter, CharecterControl
{

    public enum MODE
    {
        SINGLE, SPREAD, TRIPLE,BOMB,BOUCE
    }
    [Header("Agent properties")]
   
    public int spreadAngle;
    int atkDelay;

    [Header("Mode")]
    public MODE mode = MODE.SINGLE;
    MODE[] modeOrder = { MODE.SINGLE, MODE.SPREAD, MODE.TRIPLE };
    int modeIndex = 0;
    [Header("onPlay Boolean")]
    public bool canShoot;
    public bool isDrawBow;
    public bool isShooting;
    public bool isDodge;
    public bool isUltimate;
    public bool isWalking;
    [Header("guideline Obj")]
    public GameObject guideLine;
    public unitHitbox hitbox;
    [Header("For Archer")]
    public damageHitbox arrowPrefab;
    public damageHitbox arrowBombPrefab;
  
    public damageHitbox spreadShoot;
    public damageHitbox tripleShoot;
    public damageHitbox normalShoot;
    public damageHitbox bombShoot;
    public damageHitbox pinArrow;
    public damageHitbox bomb;
    public damageHitbox ultiModel;
    public damageHitbox ultiShoot;
    public ArcherUlti archerUlti;
    public Transform shootSpot;
    public Transform mainBody;
    public float dodgeSpeed;
    public Image modeIconUI;
    public Sprite[] modeIcons;
    Rigidbody myRigibody;
   
    [Header("Archer Arrow")]
    public int spreadShotArrow=3;
    public int mutiShotArrow=3;
    public int pinArrowNum = 1;
    public bool canDodgeShot;
    float delayTime;
    [Header("Delay After Shoot")]

    public float shootSpeed;
    [Tooltip("0=Single,1=triple,2=spread,3=bomb")]
    public float[] delayTimeOrder = new float[4];
    const int IDLE = -1, RUN = 2, SHOOT = 3, DEATH = 6, DRAW = 9, DODGE = 11;
    const string DRAWBOWcmd = "DRAWBOW", SHOOTcmd = "SHOOT", CHANGEMODEcmd = "CHANGEMODE", CHANGEMODEREcmd = "CHANGEMODERE", DODGEcmd = "DODGE",
        ULTIcmd = "ULTI", DEULTI = "DEULTI", CHARGEULTI = "CHARGEULTI", RELEASESULTI = "RELEASESULTI",
         ANALOGDOWN = "ANALOGDOWN", ANALOGUP = "ANALOGUP";
    const string SPEED = "Speed";

    // public damageHitbox normalAtkDH, swingChargeDH, blockAtkDH;
    //[SerializeField]
    [Header("particle")]
    public ParticleSystem showAvatarParticle;
    public ParticleSystem hideAvatarParticle;


    // Use this for initialization
    void Start() {
       // anim.speed = 5;
        statContol = GetComponent<StatContol>();
        if (archerUlti != null) archerUlti.gameObject.SetActive(false);
        canShoot = true;
        if (guideLine != null) guideLine.SetActive(false);
        mode = modeOrder[modeIndex];
        myRigibody = gameObject.GetComponent<Rigidbody>();
        ///
        //MODE[] temp = new MODE[modeOrder.Length + 1];
        //for (int i = 0; i < modeOrder.Length; i++)
        //{
        //    temp[i] = modeOrder[i];
        //}
        //temp[modeOrder.Length] = MODE.BOMB;
        //modeOrder = temp;
        //Debug.Log(modeOrder.Length);
    }

    // Update is called once per frame
    void Update() {
        if (id != -1)
        {
            x = CrossPlatformInputManager.VirtualAxisReference("Horizontal_" + id).GetValue;
            y = CrossPlatformInputManager.VirtualAxisReference("Vertical_" + id).GetValue;

            if (!isDeath)
            {

                if ((x != 0.00f || y != 0.00f) && !isShooting && !isDodge && !isUltimate)
                {
                    if (anim.speed != 1) anim.speed = 1;
                    //if (!isDrawBow)
                    //{
                    if (!isDrawBow)
                        transform.Translate(Vector3.forward * statContol.speedCurrent * Time.deltaTime);
                    else
                        transform.Translate(Vector3.forward * statContol.speedCurrent * Time.deltaTime*0.4f);
                    // anim.SetInteger("Speed", RUN);
                        setRun();
                        angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 45;
                        //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);
                        if (angle < 0) angle += 360;

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
                 
                    //}
                    //else if(isDrawBow)
                    //{
                    //    angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg)-45;
                    //    //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);
                    //    if (angle < 0) angle += 360;
                    //    //Debug.Log(angle);
                    //    transform.Translate(Quaternion.Euler(0, 495 - angle - 180, 0) * new Vector3(1,0,0) * statContol.speedCurrent * Time.deltaTime*0.35f);
                    //    // anim.SetInteger("Speed", RUN);
                    //    setRun();
                    //}
                }

                else if (isShooting || isDrawBow || isDodge || isUltimate)
                {
                    if (isDodge)
                    {
                        if (anim.speed != 1) anim.speed = 1;
                        transform.Translate(Vector3.forward * dodgeSpeed * Time.deltaTime);
                    }
                 
                }
                else
                {

                    anim.SetInteger("Speed", IDLE);
                    if (anim.speed != 1) anim.speed = 1;
                    myRigibody.velocity = Vector3.zero;
                }
            }


        }
    }


    IEnumerator delayShoot()
    {
        //Debug.Log("cooldown");
        yield return new WaitForSeconds(delayTime);
        canShoot = true;
    }

    IEnumerator TripleShoot()
    {
        //Debug.Log("cooldown");
        // damageHitbox dambox;
        unitHitbox myHitbox = gameObject.GetComponent<unitHitbox>();
        for (int i = 0; i < mutiShotArrow; i++)
        {

            spawnArrow(tripleShoot);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void shootArrowAct()
    {

        damageHitbox dambox;
        unitHitbox myHitbox = gameObject.GetComponent<unitHitbox>();
        if (!isUltimate)
        {
            switch (mode)
            {
                case MODE.SINGLE:
                    //for (int i = 0; i < pinArrowNum; i++)
                    //{
                    //    dambox = pinArrow.spawnDuplicate(shootSpot);
                    //    dambox.transform.localPosition = new Vector3(0.5f * (Mathf.Pow(-1, i)) * (Mathf.Floor(((float)i + 1) / 2)), 0, 0);
                    //}
                    spawnArrow(normalShoot);
                    break;
                case MODE.SPREAD:
                    for (int i = 0; i < spreadShotArrow; i++)
                    {
                        dambox = spawnArrow(spreadShoot);
                        spreadAngle = spreadShotArrow * 8;
                        dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * 8), 0);
                        /*dambox = Instantiate(arrowModel);
                        dambox.transform.position = shootSpot.position;
                        dambox.transform.eulerAngles = spreadShoot.transform.eulerAngles;
                        dambox.transform.eulerAngles += new Vector3(0,-(spreadAngle/2)+ (i* spreadAngle/4),0);

                        dambox.setDamage(spreadShoot.damage);
                        dambox.duplicateAfterDoneDamage(spreadShoot);
                        dambox.GetComponent<Bullet>().setSpeed(spreadShoot.speed);
                        dambox.setOwner(myHitbox);*/
                    }
                    break;
                case MODE.TRIPLE:
                    StartCoroutine(TripleShoot());
                    break;
                case MODE.BOMB:
                    damageHitbox damHB = bombShoot.spawnDuplicate(shootSpot);
                    ChainAttack CATK = damHB.gameObject.GetComponent<ChainAttack>();
                    CATK.chainList[0].model = bomb;
                    break;
            }
        }
        else if (isUltimate)
        {
            fireUltiArrow();
        }
        anim.SetBool("isTrail", false);
    }

    public damageHitbox spawnArrow(damageHitbox myModel)
    {

        damageHitbox dambox = Instantiate(arrowPrefab);
        Bullet myBulltet = dambox.GetComponent<Bullet>();
        Bullet modelBullet = myModel.GetComponent<Bullet>();
        dambox.transform.position = shootSpot.position;
        dambox.transform.eulerAngles = shootSpot.eulerAngles;
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(myModel.damage);
        dambox.duplicateAfterDoneDamage(myModel);
        myBulltet.setSpeed(modelBullet.speed);
        myBulltet.setBulletDistance(modelBullet.bulletDistance);
        myBulltet.maxUnit = modelBullet.maxUnit;
        dambox.setOwner(hitbox);
        dambox.ultiRegen = myModel.ultiRegen;
        return dambox;
    }

    public void shootPinArow()
    {
        if (canDodgeShot)
        {
            damageHitbox dambox = pinArrow.spawnDuplicate(shootSpot);
            dambox.transform.eulerAngles += new Vector3(0, 180, 0);
        }
    }
    public void setRun()
    {
        anim.SetBool("isRunning", true);
    }

    public void shootArrow()
    {


        if (canShoot && isDrawBow && !isDodge)//&& !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && anim.GetCurrentAnimatorStateInfo(0).IsName("Trail") && !isDodge)
        {
            if (guideLine != null) guideLine.SetActive(false);
            isShooting = true;
            //  isDrawBow = false;
            canShoot = false;
            //anim.SetInteger("Speed", SHOOT);
            anim.SetTrigger("ATK");
        }
        else if (canShoot && isDrawBow)
        {
            if (guideLine != null) guideLine.SetActive(false);
            isDrawBow = false;
        }


    }
    public void DrawStand()
    {
        if (!isShooting && canShoot && !isDrawBow && !isDodge)
        {
            anim.speed = shootSpeed;
            if (guideLine != null) guideLine.SetActive(true);
            isDrawBow = true;
            //anim.SetInteger("Speed", DRAW);
            anim.SetBool("isTrail", true);
        }


    }

    public void endShoot()
    {
        anim.speed = 1;
        isShooting = false;
        isDrawBow = false;
        StartCoroutine(delayShoot());
        if (isUltimate)
        {
            hideAvatar();
        }
        anim.SetInteger("Speed", IDLE);
        anim.SetBool("isTrail", false);
        mainBody.localRotation = Quaternion.identity;

    }


    public void preformDodge()
    {
        if (!isDrawBow && !isShooting && !isDodge && !anim.GetCurrentAnimatorStateInfo(0).IsName("Combo02") && !isUltimate)
        {
            isDodge = true;
            //anim.SetInteger("Speed", DODGE);
            anim.SetTrigger("DODGE");
        }
    }

    public void endDodge()
    {
        isDodge = false;

    }

    public void deActiveCol()
    {
       /// gameObject.GetComponent<Collider>().enabled = false;
        hitbox.isImmortal = true;
    }

    public void activeCol()
    {
        //gameObject.GetComponent<Collider>().enabled = true;
        hitbox.isImmortal = false;
    }

    public void changeMode()
    {
        modeIndex = (modeIndex + 1) % modeOrder.Length;
        Debug.Log("mode Index :" + modeIndex);
        mode = modeOrder[modeIndex];
        delayTime = delayTimeOrder[modeIndex];
       if(modeIconUI) modeIconUI.sprite = modeIcons[modeIndex];
    }
    public void changeModeR()
    {
        modeIndex = ((modeIndex + modeOrder.Length) - 1) % modeOrder.Length;
        //Debug.Log("mode Index :" + modeIndex);
        mode = modeOrder[modeIndex];
        delayTime = delayTimeOrder[modeIndex];
        modeIconUI.sprite = modeIcons[modeIndex];
    }
    public void showAvatar()
    {


        if (ultimateManager.isFull() && !isDrawBow && !isShooting && canShoot)
        {
            isUltimate = true;
            DrawStand();
            showAvatarParticle.Play();
            archerUlti.gameObject.SetActive(true);
        }
    }

    public void hideAvatar()
    {
        if (!isShooting && isUltimate)
        {
            isUltimate = false;
            hideAvatarParticle.Play();
            showAvatarParticle.Stop();
            archerUlti.gameObject.SetActive(false);
            guideLine.SetActive(false);
            isDrawBow = false;
            anim.SetInteger("Speed", IDLE - 1);
        }
    }

    public void shootUlti()
    {
        guideLine.SetActive(false);
        isShooting = true;
        //  isDrawBow = false;
        canShoot = false;
        //anim.SetInteger("Speed", SHOOT);
        anim.SetTrigger("ATK");
    }
    public damageHitbox fireUltiArrow()
    {
        if (!isUltimate)
            return null;

        damageHitbox dambox = Instantiate(ultiModel);
        dambox.GetComponent<Collider>().enabled = true;
        Bullet myBulltet = dambox.GetComponent<Bullet>();
        Bullet modelBullet = ultiShoot.GetComponent<Bullet>();
        dambox.transform.position = shootSpot.position;
        dambox.transform.eulerAngles = ultiShoot.transform.eulerAngles;
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(ultiShoot.damage);
        dambox.duplicateAfterDoneDamage(ultiShoot);
        myBulltet.setSpeed(modelBullet.speed);
        myBulltet.setBulletDistance(modelBullet.bulletDistance);
        dambox.setOwner(hitbox);
        dambox.ultiRegen = ultiShoot.ultiRegen;
        ultimateManager.useUltimate();
        hideAvatar();
        ultimateManager.endUltimate();
        return dambox;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////controlcharecter/////////////////////////////////////////////////////////////////
    /// </summary>
   override public void getCommand(string cmd)
    {
        if (isDeath) return;
        //Debug.Log(cmd);
        switch (cmd)
        {
            case SHOOTcmd:
                shootArrow();
                break;
            case DRAWBOWcmd:
                DrawStand();
                break;
            case CHANGEMODEcmd:
                changeMode();
                break;
            case CHANGEMODEREcmd:
                changeModeR();
                break;
            case DODGEcmd:
                preformDodge();
                break;
            case ULTIcmd:
                showAvatar();
                break;
            case DEULTI:
                hideAvatar();
                break;
            case RELEASESULTI:
                //shootUlti();
                shootArrow();
                //  fireUltiArrow();
                break;
            case "ULTIRIGHT":
                turnUlti(1);
                // hideAvatar();
                break;
            case "ULTILEFT":
                turnUlti(-1);
                // hideAvatar();
                break;
            case ANALOGDOWN:
                getAnalogDown();
                // hideAvatar();
                break;
            case ANALOGUP:
                getAnalogUp();
                // hideAvatar();
                break;
            default:
                string[] cmds = cmd.Split('-');
                //Debug.Log(cmds[1]);
                switch (cmds[0])
                {
                    case "BOWANGLE":
                        if (isDrawBow)
                        {
                            mainBody.eulerAngles = new Vector3(0, -float.Parse(cmds[1]) + 90, 0);
                        }
                        break;
                 
                }
                base.getCommand(cmd);
                break;

        }
    }

    override public void getUpSkill(int num)
    {
        Debug.Log("Arche UPaskill: " +num);
        switch (num)
        {
           case 1:
                 spreadShotArrow+=4;
                break;
            case 2:
                mutiShotArrow++;
                break;
            case 3:
                normalShoot.GetComponent<Bullet>().maxUnit++;
                break;
            case 4:

                MODE[] temp = new MODE[modeOrder.Length+1];
                for (int i = 0; i < modeOrder.Length; i++)
                {
                    temp[i] = modeOrder[i];
                }
                temp[modeOrder.Length] = MODE.BOMB;
                modeOrder = temp;
                break;
            case 5:
                shootSpeed += 2f;
                break;
            case 6:
                canDodgeShot = true;
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

    }
    public void getAnalogDown()
    {
        if (!isWalking)
        {
            isWalking = true;

        }
        else
        {
            //isWalking = false;
            preformDodge();
        }
    }
    public void getAnalogUp()
    {
        Invoke("endWalking", 0.4f);
        anim.SetBool("isRunning",false);
    }

    public void endWalking()
    {
        isWalking = false;
    }
    void turnUlti(int i)
    {
        if (isUltimate)
        {
            transform.eulerAngles += new Vector3(0, 2.0f * (float)i, 0);
        }

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
        resetAll();
        
        isDeath = true;
        anim.SetInteger("Speed", 6);
        source.PlayOneShot(deadSound);


    }
    public void resetAll()
    {
        hideAvatar();
        canShoot = true;
        isDodge = false;
        isDrawBow = false;
        endShoot();


    }
    public void ReportRevive()
    {
        isDeath = false;
        anim.SetInteger("Speed", -1);
    }


}
