using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [Header("Tracking Conditions")]
    public float viewRadius;
    public float stopRange;
    public particleControl spawnParticle;
    public float spawnParticleDelay;
    public LayerMask targetMask;
    [Space]
    public GameObject target;
    public unitHitbox myHitbox;
    public bool isDeath;
    public room roomOwner;
    public float speedPercent;
    public NavMeshAgent agent;
    public StatContol statContol;

    public bool notTruelyEnemy = false;

    void Start()
    {
        StartCoroutine("FindingTarget");
        statContol = GetComponent<StatContol>();
        if (agent != null)
        {
            if (statContol != null)
                agent.speed = statContol.baseSpeed;
            else
                agent.speed = 5f;
            agent.stoppingDistance = stopRange;
        }

    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    IEnumerator FindingTarget()
    {
        while (target == null)
        {
            yield return new WaitForSeconds(0.5f);
            FindTragets();
        }
        StartCoroutine("CheckTarget");
    }

    IEnumerator CheckTarget()
    {
        while (target != null)
        {
            yield return new WaitForSeconds(0.1f);
            if (Vector3.Distance(transform.position, target.transform.position) > viewRadius || target.GetComponent<unitHitbox>().isDeath)
            {
                target = null;
            }
            /* if(target.GetComponent<unitHitbox>().isDeath)
             {
                 target = null;
             }*/
        }
        StartCoroutine("FindingTarget");
    }

    void FindTragets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targets.Length; i++)
        {

            if (targets[i].tag == "Player")
            {
                if (!targets[i].GetComponent<unitHitbox>().isDeath)
                {
                    target = targets[i].gameObject;
                    break;
                }
            }
        }

    }

    public void ReportDeath(int playerWhoKill)
    {
        isDeath = true;
        if (roomOwner != null) roomOwner.EnemyInRoomDead();

        if (playerWhoKill != -1 && !notTruelyEnemy) //print(playerWhoKill);
            GameManagerPC.Instance.playerManagement.AddKillScore(playerWhoKill);

        EnemyControl enemyAI = gameObject.GetComponent<EnemyControl>();
        if (enemyAI != null)
        {
            enemyAI.ReportDeath();
        }
    }
    public void addSpeed(float d)
    {
        speedPercent += d;
        statContol.ChangeSpeedPercent(speedPercent);
        if (agent != null)
            agent.speed = statContol.speedCurrent;
    }
}
