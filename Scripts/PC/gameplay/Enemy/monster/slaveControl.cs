using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class slaveControl : MonoBehaviour, EnemyControl
{
    public float AtkRange,baseSpeed,speedPercent;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
	public bool isAttacking, isTrackingTarget,isReposition,isDeath,canMove;
    unitHitbox myHitbox;

    public StatContol statContol;

    AudioSource audioSource;

	//public float speed;
	// Use this for initialization
    void Start () {
        statContol = GetComponent<StatContol>();
        audioSource = GetComponent<AudioSource>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI= gameObject.GetComponent<EnemyAI>();
        myHitbox = gameObject.GetComponent<unitHitbox>();
        agent.stoppingDistance = AtkRange;

        isAttacking = false;
        isTrackingTarget = false;
        isReposition = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isDeath)
        {
            agent.SetDestination(transform.position);
            anim.SetInteger("Speed", 6);
            isTrackingTarget = false;
            return;
        }
        else if(canMove)
        {
            if (enemyAI.target != null)
            {
                
                isTrackingTarget = true;
                agent.speed = statContol.speedCurrent;
                agent.SetDestination(enemyAI.target.transform.position);
            }
            else
            {
                if (!isReposition)
                {
                    RePositon();
                }
                else if (Vector3.Distance(transform.position, agent.destination) < AtkRange + 1f)
                {
                    anim.SetInteger("Speed", 0);
                }
                isTrackingTarget = false;
                agent.speed = statContol.speedCurrent * 0.25f;
            }


            if (agent.velocity != Vector3.zero)
            {
                if (isTrackingTarget && Vector3.Distance(transform.position, enemyAI.target.transform.position) > AtkRange)
                {
                    anim.SetInteger("Speed", 2);
                }
                else if (enemyAI.target == null)
                {
                    anim.SetInteger("Speed", 1);
                }
            }
            else if (isTrackingTarget && Vector3.Distance(transform.position, enemyAI.target.transform.position) <= AtkRange + 0.25)
            {
                Attack();
                transform.LookAt(enemyAI.target.transform.position);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            }
        }
       else
        {
            agent.speed = 0;
        }

	}
    void RePositon()
    {
        if (isDeath)
        {
            ReportDeath();
            return;
        }
        isReposition = true;
        var myp = transform.position;
        agent.SetDestination(new Vector3(myp.x+ Random.Range(-5.0f, 5.0f),myp.y,myp.z+ Random.Range(-5.0f, 5.0f)));
        Invoke("ResetRepos", 5f);
    }
    void ResetRepos()
    {
        if (isDeath)
        {
            ReportDeath();
            return;
        }
        isReposition = false;
        anim.SetInteger("Speed", 0);
    }

    void stoptRepos()
    {
        if (isDeath)
        {
            ReportDeath();
            return;
        }
        anim.SetInteger("Speed", 0);
    }
    void Attack()
    {
        if(!isAttacking&& !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01")&& !myHitbox.isDeath)
        {
            isAttacking = true;
            anim.SetInteger("Speed", 3);
            Invoke("ResetAtk", 1f);
            Invoke("doAtk", 0.75f);
        }
    }
    void doAtk()
    {
        if (enemyAI.target != null)
        { unitHitbox unitHB = enemyAI.target.GetComponent<unitHitbox>();
          unitHB.takenDamage(statContol.damage,myHitbox);
        }
    }

    void ResetAtk()
    {
        isAttacking = false;
        anim.SetInteger("Speed", 0);
        //Debug.Log(transform.name + " is Atking");

    }
    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        anim.SetInteger("Speed", 6);
        isDeath = true;
	}

    public void selfDestruction()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void finishSpawning()
    {
        canMove = true;
    }

    public void PlaySound(AudioClip audioClip){
        //audioSource.PlayOneShot(audioClip);
    }

    public StatContol getCharacterStat()
    {
        return statContol;
    }
}
