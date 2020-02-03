using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SkullControl : MonoBehaviour,EnemyControl {

    public GameObject fireArea;
    public StatContol stat;
    public ParticleSystem particle;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
  
    unitHitbox myHitbox;
    public bool isAttacking, isDeath, canMove;
    public enum states
    {
        TRACKING, ATK, ROAMING, NONE
    }
    const int IDLE = -1, RUN = 2, DEATH = 6;
    public states myState = states.NONE;
    // Use this for initialization
    void Start()
    {

        stat = GetComponent<StatContol>();
        
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        myHitbox = gameObject.GetComponent<unitHitbox>();

    }


     // Update is called once per frame
    void Update () {
        if (!isDeath)
        {
            if (enemyAI.target != null)
            {
                if (myState == states.ROAMING)
                {
                    StopCoroutine(roaming());
                    Debug.Log("stop roam");
                    agent.SetDestination(transform.position);
                    myState = states.NONE;
                }
                transform.LookAt(enemyAI.target.transform.position);
                if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
                {
                    myState = states.ATK;

                }
                else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) >1f && myState != states.TRACKING )
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
                    anim.SetInteger("Speed", RUN);
                }
                else
                {
                    anim.SetInteger("Speed", IDLE);
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
   
    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        isDeath = true;
        anim.SetInteger("Speed",DEATH);
        fireArea.SetActive(false);
        particle.gameObject.SetActive(false);
    }

    public void selfDestruction()
    {
        Destroy(gameObject);
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
}
