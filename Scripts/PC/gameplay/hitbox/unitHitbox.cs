using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class unitHitbox : MonoBehaviour
{
    public int id = -1;
    public int hpMax;
    public int hpCurrent;
    public bool isDeath = false;
    public GuageRing hpRing;
    public ReviveArea reviveArea;
    CharecterControl playerCon;
    public UltimateManager ultimateManager;
    List<afterTakenDamage> afterTakenDamageList = new List<afterTakenDamage>();
    List<afterHeal> afterHealList = new List<afterHeal>();
    List<afterPickItem> afterPickItemsList = new List<afterPickItem>();

    public bool isImmunity, isImmortal;
    public DamageManager damageManager;

    public StatContol statContol;
    public SpriteRenderer pointerImg;

    AudioSource audioSource;
    Rigidbody rb;
    [Header("flash Blink")]
    public SkinnedMeshRenderer model;
    public Material blinkMesh;
    public Animator anim;

    public float blinkTime = 0.2f;
    bool isBlink = false;
    [SerializeField] private Material normalMesh;
    public Collider myCol;

    void Start()
    {
        isImmortal = false;
        myCol = gameObject.GetComponent<Collider>();
        anim = gameObject.GetComponent<Animator>();
        statContol = GetComponent<StatContol>();
        audioSource = gameObject.GetComponent<AudioSource>();
        playerCon = gameObject.GetComponent<CharecterControl>();
        if (model != null) normalMesh = model.material;
        // hpCurrent = hpMax;
        rb = gameObject.GetComponent<Rigidbody>();
        if (playerCon != null)
            ultimateManager = playerCon.GetUltiManager();

        if (gameObject.tag != "Player") id = -1;

        hpCurrent = hpMax;
        if (hpRing != null) hpRing.updateGauge(hpMax, hpCurrent);

        if (hpMax == 0 || hpCurrent == 0)
        {
            Debug.Log(gameObject.name + "<=start at 0 hp");
        }


    }
    IEnumerator blink()
    {
        if (!isBlink)
        {

            isBlink = true;
            model.material = blinkMesh;
            statContol.ChangeSpeedPercent(-1f);
            float s = anim.speed;
            anim.speed = 0;
            yield return new WaitForSeconds(blinkTime);
            model.material = normalMesh;
            anim.speed = 1;
            statContol.ChangeSpeedPercent(1f);
            isBlink = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null) rb.velocity = Vector3.zero;
    }

    public void takenDamage(int damage, unitHitbox attacker)
    {
        // use for instance that have shield
        ShieldEffectControl shieldEffect = GetComponentInChildren<ShieldEffectControl>();
        if (shieldEffect != null && !shieldEffect.isRemoved)
        {
            shieldEffect.takeDamage(damage, attacker);
            return;
        }
        //player
        if (playerCon != null) playerCon.recivceDamage(damage);
        int atdlength = afterTakenDamageList.Count;
        for (int i = 0; i < atdlength; i++)
        {
            if (afterTakenDamageList[i] != null)
                afterTakenDamageList[i].doActionATD(damage, attacker);
        }
        //
        if (isImmortal) return;
        //
        if (GameManagerPC.Instance != null)
            GameManagerPC.Instance.OnPlayerRecieveDamage(id);


        hpCurrent -= damage;
        if (model != null) StartCoroutine("blink");
        DamageTextControl.instance.CreateDamageText(damage, transform);

        if (hpRing != null) hpRing.updateGauge(hpMax, hpCurrent);
        if (hpCurrent <= 0)
        {
            hpCurrent = 0;
        }
        if (hpCurrent <= 0 && !isDeath)
        {
            if (isDeath) return;
            isDeath = true;

            if (attacker != null && attacker.tag == "Player")
                deathAction(attacker.id);
            else
                deathAction(-1);
        }

        // send stat to player
        if (statContol != null)
            statContol.PlayerStatChange();

    }

    public void takenHeal(int heal)
    {
        if (playerCon != null) playerCon.recivceHeal(heal);
        //if (isImmortal) return;

        hpCurrent += heal;
        DamageTextControl.instance.CreateHealText(heal, transform);
        int athLength = afterHealList.Count;
        for (int i = 0; i < athLength; i++)
        {
            if (afterHealList[i] != null) afterHealList[i].doActionAH();
        }

        if (hpCurrent > hpMax)
        {
            hpCurrent = hpMax;
        }
        if (hpRing != null) hpRing.updateGauge(hpMax, hpCurrent);

        // send stat to player
        if (statContol != null)
            statContol.PlayerStatChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other);
        if (other.GetComponent<damageHitbox>() != null)
        {
            damageHitbox otherDH = other.GetComponent<damageHitbox>();
            if ((otherDH.isFriendly && gameObject.tag == "Player") || (otherDH.isUnFriendly && gameObject.tag == "Enemy") && !isDeath)
            {
                if (other.tag == "Heal" && !otherDH.isDamaged(this) && hpCurrent < hpMax)
                {
                    otherDH.regisUnit(this);
                    Debug.Log("tigger enter heal" + other.name);
                    takenHeal(otherDH.damage);
                    showParticleEffect(otherDH.effect);
                }
                return;
            }
            /* if (otherDH.isUnFriendly && gameObject.tag == "Enemy")
             {
                 return;
             }*/
            if (other.tag == "Damage" && !otherDH.isDamaged(this))
            {
                otherDH.regisUnit(this);
                //                Debug.Log("tigger enter " + other.name);
                takenDamage(otherDH.damage, otherDH.ownerUnit);
                showParticleEffect(otherDH.effect);
            }
            //  else 
        }

        // deal with item
        DropItem dt = other.GetComponent<DropItem>();
        if (dt != null && gameObject.tag == "Player" && dt.CanPlayerPickItem(this))
        {

            ItemBase item = other.GetComponent<ItemBase>();
            if (item != null && !item.picked)
            {
                DamageTextControl.instance.CreateGetItemText(item.itemModel.popupWhenGetItemText, transform);
                item.picked = true;
            }

            // showParticleEffect(potion.getParticle());
            dt.getItem(this);
        }

    }

    void deathAction(int killerId)
    {
        // print("Call Dead Action from " + gameObject.name);

        myCol.enabled = false;
        // Shadow Character dead
        ShadowCharacterTracking tracking = GetComponent<ShadowCharacterTracking>();
        if (tracking != null) tracking.NotifyDead();

        if (playerCon != null && tag != "Enemy")
        {
            playerCon.ReportDeath();
            reviveArea.activeReviveArea();
            if (GameManagerPC.Instance != null && id >= 0)
            {
                // player Lose 10% of holding item
                ItemOnPlayer itemOnPlayer = GetComponent<ItemOnPlayer>();
                if (itemOnPlayer != null) itemOnPlayer.RemoveRandomItemOnPlayer(3);
                GameManagerPC.Instance.OnPlayerDeath(id);
            }

            return;
        }
        EnemyAI enemyAI = gameObject.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.ReportDeath(killerId);
        }

        DropControl dropControl = gameObject.GetComponent<DropControl>();
        if (dropControl != null)
        {
            dropControl.randomDrop();
        }

    }

    public void reviveAction()
    {
        Debug.Log(gameObject.name + "|revive");
        myCol.enabled = true;
        if (playerCon != null || tag == "Player")
        {


            playerCon.ReportRevive();
            reviveArea.deActiveReviveArea();
            takenHeal((hpMax * 40) / 100);
            isDeath = false;

            if (GameManagerPC.Instance != null)
            {
                GameManagerPC.Instance.OnPlayerRevive(id);
            }

            return;
        }


    }
    public void showParticleEffect(particleControl DamParticle)
    {
        if (DamParticle == null) return;
        particleControl newParticle = Instantiate(DamParticle);
        newParticle.transform.SetParent(transform);
        newParticle.setParticle(transform);
    }

    public void playSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



    public void setID(int i)
    {
        id = i;
        if (pointerImg != null)
        {
            switch (id)
            {
                case 0:
                    pointerImg.color = Color.red;
                    break;
                case 1:
                    pointerImg.color = Color.yellow;
                    break;
                case 2:
                    pointerImg.color = Color.blue;
                    break;
                case 3:
                    pointerImg.color = Color.green;
                    break;
            }
            Color tmp = pointerImg.color;
            tmp.a = 0.2f;
            pointerImg.color = tmp;

        }
    }


    public void addAfterTakenDamage(afterTakenDamage atd)
    {
        if (!afterTakenDamageList.Contains(atd))
        {
            afterTakenDamageList.Add(atd);
        }
    }

    public void addAfterHeal(afterHeal ath)
    {
        if (!afterHealList.Contains(ath))
        {
            afterHealList.Add(ath);
        }
    }


    public void addAfterPickItem(afterPickItem at)
    {
        if (!afterPickItemsList.Contains(at))
        {
            afterPickItemsList.Add(at);
        }
    }

    public void removeAfterTakenDamage(afterTakenDamage atd)
    {
        if (afterTakenDamageList.Contains(atd))
        {
            afterTakenDamageList.Remove(atd);
        }
    }


    public void removeAfterHeal(afterHeal ath)
    {
        if (afterHealList.Contains(ath))
        {
            afterHealList.Add(ath);
        }
    }


    public void removeAfterPickItem(afterPickItem at)
    {
        if (afterPickItemsList.Contains(at))
        {
            afterPickItemsList.Remove(at);
        }
    }

    public void ReportUltiFull()
    {
        if (playerCon != null && tag == "Player")
        {
            playerCon.ReportUltiFull();
        }
    }

}
