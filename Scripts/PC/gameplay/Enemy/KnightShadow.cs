using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class KnightShadow : MonoBehaviour, EnemyControl, afterTakenDamage
{
    public KnightControl knightControl;
    public float AtkRange;
    NavMeshAgent agent;
    public Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isDeath, canMove;
    private bool isTakenDamage;

    public enum states
    {
        TRACKING, ATK, ROAMING, NONE
    }
    const int IDLE = -1, RUN = 2, SHOOT = 3, DEATH = 6, DRAW = 9, DODGE = 11;
    public states myState = states.NONE;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        enemyAI = gameObject.GetComponent<EnemyAI>();
        enemyAI.myHitbox.addAfterTakenDamage(this);

    }

    void Update()
    {
        if (!isDeath)
        {
            if (enemyAI.target != null)
            {
                if (myState == states.ROAMING)
                {
                    StopCoroutine("roaming");
                    Debug.Log("stop roam");
                    agent.SetDestination(transform.position);
                    myState = states.NONE;
                }
                //
                if (!knightControl.ultimateManager.isUltimating)//freez rotation case
                {
                    transform.LookAt(enemyAI.target.transform.position);
                }
                //
                if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < AtkRange + 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
                {
                    myState = states.ATK;
                    StartCoroutine("attacking");

                }
                else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange + 1f && myState != states.TRACKING)
                {
                    //StopCoroutine(attacking());
                    myState = states.TRACKING;
                }

                if (myState == states.TRACKING)
                {
                    if(!knightControl.isPrefromUlti) agent.SetDestination(enemyAI.target.transform.position);
                }
                else if (myState == states.ATK)
                {
                   if(!knightControl.isPrefromUlti) agent.SetDestination(transform.position);

                }
            }
            else
            {

                if (myState != states.ROAMING)
                {
                    Debug.Log("roam");
                    StartCoroutine(roaming());
                    myState = states.ROAMING;
                }
            }
            if (myState != states.ATK && agent.velocity != Vector3.zero)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);

            }

        }
    }

    IEnumerator roaming()
    {
        while (true)
        {
            if(!knightControl.isPrefromUlti)
            {
                RePositon();
                yield return new WaitForSeconds(5f);
                agent.SetDestination(transform.position);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator attacking()
    {
        while (myState == states.ATK)
        {
            if (knightControl.ultimateManager.isFull())
            {
                 knightControl.preformUltimate();
                while (knightControl.isPrefromUlti)
                {
                    yield return new WaitForSeconds(0.125f);
                    knightControl.preformUltimate();
                    if (isDeath)
                    {
                        ReportDeath();
                        break;
                    }
                }
            }
            knightControl.performAttack();
            yield return new WaitUntil(() => !knightControl.isAtk);
            if(isTakenDamage)
            {
                knightControl.performBlock();
                isTakenDamage = false;
            }

            while (knightControl.isBlocking)
            {
                if (knightControl.ultimateManager.isFull())
                {
                    isTakenDamage = false;
                }
                yield return new WaitForSeconds(1f);
                knightControl.performAttack();
                yield return new WaitUntil(() => !knightControl.isAtk);
                if (!isTakenDamage)
                {
                    knightControl.performUnBlock();
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    isTakenDamage = false;
                }
                if (isDeath)
                {
                    ReportDeath();
                    break;
                }

            }
            if(isDeath)
            {
                ReportDeath();
                break;
            }

        }
    }
    public void ReportDeath()
    {
        isDeath = true;
        gameObject.GetComponent<Collider>().enabled = false;
        knightControl.ReportDeath();
        // anim.SetInteger("Speed", 6);
        

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
    public StatContol getCharacterStat()
    {
        return GetComponent<StatContol>();
    }
    public void doActionATD(int damageTaken, unitHitbox takenFrom)
    {
        if (knightControl.stamina > knightControl.stamina * 0.2f)
        {
            isTakenDamage = true;
        }
    }
}
