using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class LancerShadow : MonoBehaviour ,EnemyControl{
    public LancerControl control;
    public float AtkRange;
    NavMeshAgent agent;
    private int tempTurnSpeed;
    public Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isDeath, canMove;
    public enum states
    {
        TRACKING, ATK, ROAMING, NONE,RUSH
    }
    const int IDLE = -1, RUN = 2, SHOOT = 3, DEATH = 6, DRAW = 9, DODGE = 11;
    public states myState = states.NONE;

    [HideInInspector] public StatContol stat;
    // Use this for initialization
    void Start()
    {
        stat = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
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
                    StopCoroutine("roaming");
                    Debug.Log("stop roam");
                    agent.SetDestination(transform.position);
                    myState = states.NONE;
                    control.mainBody.LookAt(agent.destination);
                }
                if (!control.ultimateManager.isUltimating) control.mainBody.LookAt(enemyAI.target.transform.position);
                if (myState != states.RUSH && Vector3.Distance(transform.position, enemyAI.target.transform.position) < AtkRange + 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
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
                    if (control.ultimateManager.isUltimating)
                    {
                       
                    }
                    else
                    {
                        agent.SetDestination(transform.position);
                    }

                }
                else if(myState == states.RUSH)
                {
                    transform.Translate(control.mainBody.forward * control.statContol.speedCurrent * Time.deltaTime * (1f + control.RushspeedUpfactor));
                }
            }
            else
            {

                if (myState != states.ROAMING)
                {
                    control.AtkBtndown();
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
                control.setUlti();
                myState = states.RUSH;
                while (control.ultimateManager.isUltimating)
                {
                    control.mainBody.LookAt(enemyAI.target.transform.position);
                    yield return new WaitForSeconds(2f);
                }
                myState = states.ATK;

            }
            else
            {
                control.AttackRelease();
                control.AtkBtndown();
            }
            yield return new WaitForSeconds(0.5f);
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
        gameObject.GetComponent<Collider>().enabled = false;
        // anim.SetInteger("Speed", 6);
        control.ReportDeath();
        isDeath = true;

    }

    public void setRush()
    {

    }
}
