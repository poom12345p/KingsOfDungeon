using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class KnightControl_BU : MonoBehaviour
{

    public float turnspeed, speed, angle;
    public int id = -1;
    public int baseStamina;
    Animator anim;
    int atkDelay;
   
    float x, y;
    bool isAtk,isReceiving,isBlocking,isCharging;
    int nextAct = 0;
    [SerializeField] int chargState=0;

    public GameObject normalAtk;
    public unitHitbox hitbox;
    public GuageRing staminaGuage;
    public damageHitbox normalAtkDH, swingChargeDH;
    [SerializeField]
    float stamina, maxStamina;
    const float baseStaRegen = 3, baseStaBlockRegen = 1, baseStaBlockUse = 5, baseChargeUse=3;
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //SOUND
    AudioSource source;
    int checkSound;
    // Use this for initialization
    void Start()
    {
        stamina = maxStamina = baseStamina;
        updateStamina();
        hitbox = gameObject.GetComponent<unitHitbox>();
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
 
    void FixedUpdate()
    {
        if (id != -1)
        {
            x = CrossPlatformInputManager.VirtualAxisReference("Horizontal_" + id).GetValue;
            y = CrossPlatformInputManager.VirtualAxisReference("Vertical_" + id).GetValue;
            if (!isAtk && !isBlocking)
            {
                if ((x != 0.00f || y != 0.00f) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
                {
                   
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    anim.SetInteger("Speed", 2);
               
                }
                else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
                {
                    anim.SetInteger("Speed", -1);
                }
            }
            if ((x != 0.00f || y != 0.00f) )
            {
                angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg)-45+90;
                //Debug.Log("x:" + x + "|y:" + y + "|angle:" + angle);
                if (angle < 0) angle += 360;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 495 - angle - 180, 0), Time.deltaTime * turnspeed);
            }
            if (stamina < maxStamina && chargState==0)
            {
                if (isBlocking)
                {
                    stamina += baseStaBlockRegen *Time.deltaTime;
                }
                else
                {
                   stamina += baseStaRegen * Time.deltaTime;
                }
                updateStamina();
            }
            else if(chargState>0)
            {
                stamina -= chargState * Time.deltaTime;
                updateStamina();
            }
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

    public void delayAtkCount()
    {
        if (nextAct == 0)
        {
            isAtk = false;
        }
        else
        {
            isReceiving = false;
            setNormalAtk();
        }
        anim.SetInteger("Speed", nextAct);
        nextAct = 0;

    }

    public void delayReReceiving()
    {
        isReceiving = true;

    }
    public void getCommand(string cmd)
    {
        switch (cmd)
        {
            case "ATTACK":
                performAttack();
                break;
            case "BLOCK":
                performBlock();
               // Attack();
                break;
            case "UNBLOCK":
                performUnBlock();
                break;
            case "CHARGEUP":
             chargeUP();
                break;
            case "CHARGERELEASE":
              chargeRelease();
                break;
        }
    }
    public void setID(int _id)
    {
        id = _id;
        Debug.Log("Knight get id: " + id);
    }
    void performAttack()
    {
        Debug.Log("preformATk");
        if (isCharging) return;
        if (!isAtk)
        {
            isAtk = true;
            isReceiving = false;
            if (anim.GetInteger("Speed") < 3)
            {
                anim.SetInteger("Speed", Random.Range(3, 5));
            }
            else
            {
                anim.SetInteger("Speed", 3 + ((anim.GetInteger("Speed") - 2) % 2));
            }
            anim.SetFloat("Accel", 1f);
            setNormalAtk();
        }
        else if (nextAct == 0&& isReceiving)
        {
            nextAct = 3 + ((anim.GetInteger("Speed") - 2) % 2);
        }
       
    }

    void performAtk3C()
    {
        Debug.Log("preformATk");
        isAtk = true;
        anim.SetInteger("Speed", 10);
        anim.SetFloat("Accel", 1f);
    }
    void performEndAtk()
    {
        isAtk = false;
        anim.SetInteger("Speed", 0);
    }
    void setNormalAtk()
    {
        Invoke("ActiveNormalAtkHitbox", 0.4f);
        Invoke("deActiveNormalAtkHitbox", 0.75f);
        Invoke("delayReReceiving", 0.75f);
        Invoke("delayAtkCount", 1.0f);
    }
    public void deActiveNormalAtkHitbox()
    {
        normalAtk.GetComponent<damageHitbox>().deActiveHitbox();
    }
    public void ActiveNormalAtkHitbox()
    {
        normalAtkDH.activeHitbox();
    }

    void performBlock()
    {
        if (stamina > 0)
        {
            isBlocking = true;
            anim.SetInteger("Speed", 5);
            hitbox.isImmortal = true;
            updateStamina();
        }
    }
    void performUnBlock()
    {
        isBlocking = false;
        anim.SetInteger("Speed", 0);
        hitbox.isImmortal = false;
        updateStamina();
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
        else if(stamina <= maxStamina)
        {
            staminaGuage.gaugeImage.enabled = true;
        }
        
         staminaGuage.updateGauge( maxStamina,stamina);
       
    }
    public void recivceDamage(int damage)
    {
        if(isBlocking)
        {
            Debug.Log("stamina decreas");
            stamina -= baseStaBlockUse;
            updateStamina();
        }
    }
    public void chargeUP()
    {
        if(chargState<3)
        {
            chargState++;
            isCharging = true;
        }
       
    }
    public void chargeRelease()
    {
        switch (chargState)
        {
            case 0:
                performAttack();
                return;
            case 1:
                break;
            case 2:
                break;
            case 3:
                performAtk3C();
                break;
        }
        //isAtk = true;
        // anim.SetInteger("Speed", 10);
        // Invoke("delayAtkCount", 1.0f);
        isCharging = false;
        chargState = 0;
    }

    public void hideHitbox(string AtkTag)
    {
       damageHitbox myDH;
        switch (AtkTag)
        {
            case "NORMAL":
                myDH = normalAtkDH;
                break;
            case "SWING":
                myDH = swingChargeDH;
                break;
            default:
                return;

        }
        myDH.deActiveHitbox();
    }

    public void activeHitbox(string AtkTag)
    {
        damageHitbox myDH;
        switch (AtkTag)
        {
            case "NORMAL":
                myDH = normalAtkDH;
                break;
            case "SWING":
                myDH = swingChargeDH;
                break;
            default:
                return;

        }
        myDH.activeHitbox();
    }

}
