using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArcherShadow : MonoBehaviour, EnemyControl
{


    public ArcherControl archerControl;
    public float AtkRange;
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
    void Start()
    {
        stat = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();

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
                }
                if (!archerControl.isDodge) transform.LookAt(enemyAI.target.transform.position);
                if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < AtkRange + 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
                {
                    myState = states.ATK;
                    StartCoroutine("attacking");

                }
                else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange + 1f && myState != states.TRACKING && !archerControl.isShooting)
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
                    if (!archerControl.isDodge) agent.SetDestination(transform.position);

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

            if (!archerControl.isDrawBow && !archerControl.isShooting)
            {
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
    }

    IEnumerator attacking()
    {
        int loopCount = 0;
        while (myState == states.ATK)
        {
            if (archerControl.ultimateManager.isFull())
            {
                archerControl.showAvatar();
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                if (!isDeath) archerControl.DrawStand();
                yield return new WaitForSeconds(0.1f);
            }
            if (!isDeath) archerControl.shootArrow();

            yield return new WaitUntil(() => archerControl.canShoot);
            loopCount++;
            if (loopCount == 10)
            {
                loopCount = 0;
                ChangeMode();
            }
            if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < 4f)
            {
                archerControl.preformDodge();
                transform.eulerAngles += new Vector3(0, 180, 0);
                agent.SetDestination(transform.position + (Vector3.forward * 5f));
                yield return new WaitUntil(() => !archerControl.isDodge);
            }
        }
    }
    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        archerControl.ReportDeath();
        // anim.SetInteger("Speed", 6);
        isDeath = true;

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
    public void ChangeMode()
    {
        archerControl.changeModeR();
    }
    public void selfDestruction()
    {
        Destroy(gameObject);
    }

    public StatContol getCharacterStat()
    {
        return stat;
    }
}
