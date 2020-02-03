using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardShadow : MonoBehaviour, EnemyControl
{
    public WizardControl control;
    public float AtkRange,CycloneRange;
    NavMeshAgent agent;
    public Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isDeath, canMove;
    public enum states
    {
        TRACKING, ATK, ROAMING, NONE
    }
    const int IDLE = -1, RUN = 2, SHOOT = 3, DEATH = 6, DRAW = 9, DODGE = 11;
    public states myState = states.NONE;

    [HideInInspector] public StatContol stat;
    // Use this for initialization
    void Start () {
        stat = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
    }
	
	// Update is called once per frame
	void Update () {
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
                transform.LookAt(enemyAI.target.transform.position);
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
                    StartCoroutine(roaming());
                    myState = states.ROAMING;
                }
            }
            if (myState != states.ATK && agent.velocity != Vector3.zero)
            {
                anim.SetInteger("Speed", 2);
            }
            else
            {
                anim.SetInteger("Speed", -1);

            }

           
        }
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

        while (myState == states.ATK)
        {
            if (control.ultimateManager.isFull())
            {

                yield return new WaitUntil(() => !control.isAtk);
                control.startCast();
                if (!control.isCast)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                yield return new WaitUntil(() => control.chargeCount == 4);
                control.stopCast();
                yield return new WaitForSeconds(1f);


            }
           else
            {
                yield return new WaitUntil(()=> !control.isAtk );
                control.startCast();
                if(!control.isCast)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                if(Vector3.Distance(transform.position, enemyAI.target.transform.position) < CycloneRange)
                {
                    yield return new WaitUntil(() =>control.chargeCount==2);
                    
                }
                else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < AtkRange)
                {
                    if (control.chargeCount > 1)
                    {
                        control.stopCast();
                    }
                    else
                    {
                        yield return new WaitUntil(() => control.chargeCount == 1);
                    }
                }
                control.stopCast();
            }
        }
    }
    public StatContol getCharacterStat()
    {
        return stat;
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

    public void selfDestruction()
    {
        Destroy(gameObject);
    }
    public void ReportDeath()
    {
        isDeath = true;

        gameObject.GetComponent<Collider>().enabled = false;
        // anim.SetInteger("Speed", 6);
        control.ReportDeath();
       
    }

}
