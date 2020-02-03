using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LaserTotem : MonoBehaviour,EnemyControl {
    public GameObject guideline;
    public ContinueDamage laserHitbox;
    public Transform body;
    public ParticleSystem chargeParticle,laserParticle;
    NavMeshAgent agent;
    Animator anim;
    EnemyAI enemyAI;
    public LineRenderer laserLine;
    public StatContol stat;
    public bool  isDeath;
    public AudioSource audioSource;
    public AudioClip chargeSound,laserSound;


    public enum states
    {
        TRACKING, ATK, NONE,OPEN
    }
    public states myState = states.NONE;
    public float periodTime,changeSpeed,moveSpeed;
    public float cooldownTime;
    // Use this for initialization
    void Start()
    {
        laserParticle.Stop();
        stat = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        laserHitbox.stopBlinking();
    }
	
	// Update is called once per frame
    IEnumerator openLaser()
    {
        float i = 0;
        laserHitbox.stopBlinking();
        yield return new WaitForSeconds(cooldownTime);
        myState = states.ATK;
        laserParticle.Play();
        laserHitbox.startBlinking();
        audioSource.Play();
        while (i<=1.0f)
        {
            yield return new WaitForFixedUpdate();
            {
                laserLine.SetWidth(i, i);
                i += changeSpeed * Time.deltaTime;
            }
        }
        yield return new WaitForSeconds(periodTime);
        laserParticle.Stop();
        audioSource.Stop();
        while (i >0f)
        {
            yield return new WaitForFixedUpdate();
            {
                laserLine.SetWidth(i, i);
                i -= changeSpeed * Time.deltaTime;
            }
        }
        laserHitbox.stopBlinking();
        myState = states.NONE;



    }
	void Update () {
        if (!isDeath)
        {
            if (enemyAI.target != null)
            {
                if (myState == states.NONE )
                {
                    StartCoroutine(openLaser());
                    myState = states.TRACKING;
                }

                guideline.SetActive(true);
                if (myState == states.TRACKING)
                {
                    Transform target = enemyAI.target.transform;
                    var lookPos = target.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    body.rotation = Quaternion.Slerp(body.rotation, rotation, Time.deltaTime * moveSpeed);
                }

            }
            else
            {
                guideline.SetActive(false);
                
            }
        }
    }

    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        laserHitbox.gameObject.SetActive(false);
        guideline.SetActive(false);
        body.gameObject.SetActive(false);
        Invoke("selfDestruction", 0.5f);
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
