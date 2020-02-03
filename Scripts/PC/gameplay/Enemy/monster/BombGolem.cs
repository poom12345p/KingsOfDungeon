using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BombGolem : MonoBehaviour,EnemyControl {

    public float AtkRange;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isDeath, canMove;
    public damageHitbox bombBase, bombPrefab;
    public unitHitbox hitbox;
    public AudioClip BoomSound;
    public enum states
    {
        TRACKING, ATK, ROAMING, NONE
    }
    public states myState = states.NONE;
    const int IDLE = -1, RUN = 2, BOMB = 5, DEATH = 6;

    public StatContol statContol;
    // Use this for initialization
    void Start () {
        statContol = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
    }
    IEnumerator roaming()
    {
        while (true)
        {
            RePositon();
            yield return new WaitForSeconds(5f);
            agent.SetDestination(transform.position);
            yield return new WaitForSeconds(3f);
        }
    }
    IEnumerator attacking()
    {
        anim.speed = 4;
        anim.SetInteger("Speed", BOMB);
        while (true)
        {
            yield return new WaitForSeconds(1f);
            anim.speed += 2;
            if(anim.speed ==12)
            {
                spawnBomb(bombBase, bombPrefab);
                anim.speed = 1;
                hitbox.playSound(BoomSound);
                hitbox.takenDamage(hitbox.hpMax,bombBase.ownerUnit);
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (!isDeath)
        {
            if (enemyAI.target != null)
            {
                if (myState == states.ROAMING)
                {
                    StopCoroutine(roaming());
                    //enemyAI.addSpeed(0.9f);
                    Debug.Log("stop roam");
                    agent.SetDestination(transform.position);
                    myState = states.NONE;
                }
                transform.LookAt(enemyAI.target.transform.position);
                if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < AtkRange + 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
                {
                    myState = states.ATK;
                    StartCoroutine(attacking());

                }
                else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange + 2f && myState != states.TRACKING )
                {
                    StopCoroutine(attacking());
                    anim.speed = 1;
                    myState = states.TRACKING;
                }

                if (myState == states.TRACKING)
                {
                    StopCoroutine(attacking());
                    anim.speed = 1;
                    agent.SetDestination(enemyAI.target.transform.position);
                }
                else if (myState == states.ATK)
                {
                    agent.SetDestination(transform.position);
                }
            }
            else
            {

                if (myState != states.ROAMING)
                {
                    Debug.Log("roam");
                    //enemyAI.addSpeed(-0.9f);
                    StopCoroutine(attacking());
                    StartCoroutine(roaming());
                    anim.speed = 1;
                    myState = states.ROAMING;
                }
            }

                if (myState != states.ATK && agent.velocity != Vector3.zero)
                {
                    anim.SetInteger("Speed", RUN);
                }
                else if(myState != states.ATK)
                {
                    anim.SetInteger("Speed", IDLE);
                }
        }

    }

    void RePositon()
    {
        if (isDeath)
        {
            ReportDeath();
            return;
        }
        var myp = transform.position;
        agent.SetDestination(new Vector3(myp.x + Random.Range(-10.0f, 10.0f), myp.y, myp.z + Random.Range(-10.0f, 10.0f)));

        // Invoke("ResetRepos", 5f);
    }

    public void ReportDeath()
    {
        anim.speed = 1;
        gameObject.GetComponent<Collider>().enabled = false;
        anim.SetInteger("Speed", 6);
        agent.speed = 0;
        isDeath = true;
        //Invoke("selfDestruction", 3f);
    }

    public void selfDestruction()
    {
        Destroy(gameObject);
    }

    public damageHitbox spawnBomb(damageHitbox myBase, damageHitbox myPrefab)
    {
       
        damageHitbox dambox = Instantiate(myPrefab);
        dambox.transform.position = transform.position;
        dambox.transform.eulerAngles = myBase.transform.eulerAngles;
        // dambox.transform.eulerAngles += new Vector3(0, -(spreadAngle / 2) + (i * spreadAngle / 4), 0);
        dambox.setDamage(myBase.damage);
        dambox.duplicateAfterDoneDamage(myBase);
        dambox.setOwner(hitbox);
        dambox.ultiRegen = myBase.ultiRegen;
        return dambox;
    }

    public StatContol getCharacterStat()
    {
        return statContol;
    }
}
