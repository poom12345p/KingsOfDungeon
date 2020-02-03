using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DarkArcherControl : MonoBehaviour, EnemyControl
{
    public damageHitbox arrowModel;
    public damageHitbox normalShoot;
    public ArcherControl archerControl;
    public float AtkRange;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
    public bool isAttacking, isDeath, canMove;
    public enum states
    {
        TRACKING,ATK,ROAMING,NONE
    }
    const int IDLE = -1, RUN = 2, SHOOT = 3, DEATH = 6, DRAW = 9, DODGE = 11;
    public states myState = states.NONE;

    [HideInInspector]public StatContol stat;
    // Use this for initialization
    void Start () {
        stat = GetComponent<StatContol>();
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
        while (myState == states.ATK)
        {
           if(!isDeath) archerControl.DrawStand();
            yield return new WaitForSeconds(1f);
            if (!isDeath) archerControl.shootArrow();
            yield return new WaitUntil(()=>archerControl.canShoot);
        }
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
                else if(Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange + 1f && myState != states.TRACKING && !archerControl.isShooting)
                {
                    //StopCoroutine(attacking());
                    myState = states.TRACKING;
                }

                if (myState == states.TRACKING)
                {
                    agent.SetDestination(enemyAI.target.transform.position);
                }
                else if(myState == states.ATK)
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

            if (!archerControl.isDrawBow && !archerControl.isShooting)
            {
                if (myState != states.ATK && agent.velocity != Vector3.zero)
                {
                    anim.SetBool("isRunning", true);
                }
                else
                {
                    anim.SetBool("isRunning",false);
                }
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

    public virtual void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        anim.SetInteger("Speed", 6);
        isDeath = true;
        Invoke("selfDestruction", 3f);
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
