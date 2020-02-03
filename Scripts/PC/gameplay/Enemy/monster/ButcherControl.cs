using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ButcherControl : MonoBehaviour,EnemyControl
{
    public float AtkRange, Speed;
    public damageHitbox normalRight, normalLetf, combo5, combo3, knife;
    public GameObject normalArea, combo5Area, combo3Area, knifeArea;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isTrackingTarget, isReposition,isDeath,isOpen =true;
    const int speedRun = 5, speedWalk = 2, speedLeft = 4, speedRight = 3, speedCombo3 = 6, speedCombo5 = 8, speedDeath = -1;
    [SerializeField] int atkPoint = 0,nextAtk;
    [SerializeField] bool isAction,isComand;
    // Use this for initialization

    public AudioClip butcherWarCry;
    public AudioClip butcherWalk;

    public StatContol statContol;

    AudioSource _audioSource;
    void Start()
    {
        statContol = GetComponent<StatContol>();
        isAction =isComand= false;
        setnextAct();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        _audioSource = GetComponent<AudioSource>();
        agent.stoppingDistance = AtkRange;
        agent.speed = statContol.speedCurrent;
        isAttacking = false;
        isTrackingTarget = false;
        isReposition = false;

        //_audioSource.PlayOneShot(butcherWarCry);
    }
    public enum damageHitboxTag
    {
        NORMALLEFT, NORMALRIGHT, COMBO5, COMBO3
    }

    public void PlaySound(AudioClip clip){
        _audioSource.PlayOneShot(clip);
    }

    void FixedUpdate()
    {
        if (isOpen) return;
        if (isDeath)
        {
            agent.SetDestination(transform.position);
            isTrackingTarget = false;
            return;
        }
        if (enemyAI.target != null)
        {
            
            agent.SetDestination(enemyAI.target.transform.position);
            isTrackingTarget = true;
            isReposition = false;
            
        }
        else
        {
            isComand = false;
            if (!isReposition)
            {
                RePositon();
            }
            else if (Vector3.Distance(transform.position, agent.destination) < AtkRange + 1f)
            {
                anim.SetInteger("Speed", -1);
            }
            isTrackingTarget = false;
           // agent.speed = Speed * 0.25f;
        }


        if (agent.velocity != Vector3.zero && !isAction)
        {
            if (isTrackingTarget && Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange)
            {
                anim.SetInteger("Speed", speedRun);
                if (isComand)
                {
                    isComand = false;
                }
            }
            else if (enemyAI.target == null)
            {
                anim.SetInteger("Speed", speedWalk);
            }
           
        }
        else if (isTrackingTarget && Vector3.Distance(transform.position, enemyAI.target.transform.position) <= AtkRange + 0.25)
        {
            //atk
            if (!isAction)
            {
                //isComand = true;
                anim.SetInteger("Speed", nextAtk);
            }
            //
            if (!isAction)
            {
                transform.LookAt(enemyAI.target.transform.position);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            }
                
        }

    }
    void RePositon()
    {
        isReposition = true;
        var myp = transform.position;
        agent.SetDestination(new Vector3(myp.x + Random.Range(-5.0f, 5.0f), myp.y, myp.z + Random.Range(-5.0f, 5.0f)));
        Invoke("ResetRepos", 5f);
    }
    void ResetRepos()
    {
        isReposition = false;
        anim.SetInteger("Speed", -1);
    }

    void stoptRepos()
    {
        anim.SetInteger("Speed", -1);
    }
    void actvieDamageHItbox(damageHitboxTag dt)
    {
        damageHitboxFitter(dt).activeHitbox();
    }

    void deActvieDamageHItbox(damageHitboxTag dt)
    {
        damageHitboxFitter(dt).deActiveHitbox();
    }
    public damageHitbox damageHitboxFitter(damageHitboxTag dt)
    {
        damageHitbox hitbox = null;
        switch (dt)
        {
            case damageHitboxTag.NORMALLEFT:
                hitbox = normalLetf;
                break;
            case damageHitboxTag.NORMALRIGHT:
                hitbox = normalRight;
                break;
            case damageHitboxTag.COMBO5:
                hitbox = combo5;
                break;
            case damageHitboxTag.COMBO3:
                hitbox = combo3;
                break;
        }
        return hitbox;
    }

    public void selfDestruction()
    {
        gameObject.SetActive(false);
    }

    public void ReportDeath()
    {
       // gameObject.SetActive(false);
        isDeath = true;

        anim.SetInteger("Speed", 9);
    }
    public void startAction()
    {
        isAction = true;
        agent.speed = 0;
    }
    public void endAction()
    {
        anim.SetInteger("Speed", 0);
        //delayEndAct();
        Invoke("delayEndAct", 0.5f);
    }
    public void delayEndAct()
    {
        isComand = false;
        isAction = false;
        agent.speed = statContol.speedCurrent;
        if (atkPoint < 7)
        {
            atkPoint++;
        }
        else if (atkPoint >= 7)
        {
            atkPoint = 0;
        }
        setnextAct();
    }
    void setnextAct()
    {
            if (atkPoint < 7)
            {
                if (atkPoint == 3)
                {
                    nextAtk = speedCombo5;
                }
                else
                {

                    int a = Random.Range(0, 2);
                    switch (a)
                    {
                        case 0:
                            nextAtk = speedRight;
                            break;
                        case 1:
                            nextAtk = speedLeft;
                            break;
                    }
                }
            }
            else
            {
                nextAtk = speedCombo3;
            }
    }



    void PlayWalkSound(){
        _audioSource.loop = true;
        _audioSource.clip = butcherWalk;
        _audioSource.Play();
    }

    public StatContol getCharacterStat()
    {
        return statContol;
    }
    public void endOpen()
    {
        isOpen = false;
    }

    public void throwKnife()
    {
        knife.spawnDuplicate(knife.transform);
    }

    public void hideAraa(damageHitboxTag dt)
    {
        GameObject hitbox = null;
        switch (dt)
        {
            case damageHitboxTag.NORMALRIGHT:
                hitbox = normalArea;
                break;
            case damageHitboxTag.NORMALLEFT:
                hitbox = knifeArea;
                break;
            case damageHitboxTag.COMBO5:
                hitbox = combo5Area;
                break;
            case damageHitboxTag.COMBO3:
                hitbox = combo3Area;
                break;
        }
        hitbox.SetActive(false);
    }

    public void showAraa(damageHitboxTag dt)
    {
        GameObject hitbox = null;
        switch (dt)
        {
            case damageHitboxTag.NORMALRIGHT:
                hitbox = normalArea;
                break;
            case damageHitboxTag.NORMALLEFT:
                hitbox = knifeArea;
                break;
            case damageHitboxTag.COMBO5:
                hitbox = combo5Area;
                break;
            case damageHitboxTag.COMBO3:
                hitbox = combo3Area;
                break;
        }
        hitbox.SetActive(true);
    }
}
