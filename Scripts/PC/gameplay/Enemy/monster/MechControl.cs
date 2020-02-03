using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechControl : MonoBehaviour {

   
    Animator anim;
    EnemyAI enemyAI;
    StatContol stat;
    NavMeshAgent agent;
    public enum states
    {
        TRACKING, ATK, ROAMING, NONE,JUMP
    }
    public enum atkTag
    {
       NORMAL,JUMP,FOOT
    }
    [System.Serializable]
    public struct atkObj
    {
        public damageHitbox hitbox;
        public ParticleSystem particle;
        public GameObject alertArea;

    }

    public atkObj footAtk, jumpAtk, normalAtk;
    const int IDLE = -1, RUN = 2, NORMALATK= 4, DEATH = 6, JUMP = 7, FOOTATK = 5;
    public states myState = states.NONE;
    public bool isAtk = false,isJump,isDeath;
    bool isOpen = true;
    public float maxjumpstack = 18;
    public float jumpRange,jumpSpeed;
    public AudioSource skillAudio;
   [SerializeField]private float jumpStack =0;
    // Use this for initialization
    void Start() {
        stat = GetComponent<StatContol>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
    }

    // Update is called once per frame

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
        int i = 0;
        while (myState == states.ATK &&!enemyAI.isDeath)
        {
            
            Debug.Log("i :" + i);
            isAtk = true;
            if (i<3 )
            {
                if (!enemyAI.isDeath)
                    prefromAnim(NORMALATK);
                else
                    prefromAnim(DEATH);
                i++;
            }
           else if(i==3)
            {
                if (!enemyAI.isDeath)
                    prefromAnim(FOOTATK);
                else
                    prefromAnim(DEATH);
                i = 0;
            }

           yield return new WaitUntil(() => !isAtk);
        
        }
    }

    void Update()
    {

		if (!enemyAI.isDeath && !isOpen)
		{
			if (jumpStack < maxjumpstack)
			{
				jumpStack += 1 * Time.deltaTime;
			}
			else if (jumpStack > maxjumpstack)
			{
				jumpStack = maxjumpstack;
			}
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
				if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < enemyAI.stopRange + 0.5f && myState != states.ATK && agent.velocity == Vector3.zero)
				{
					myState = states.ATK;
					StartCoroutine("attacking");

				}
				else if (Vector3.Distance(transform.position, enemyAI.target.transform.position) > enemyAI.stopRange + 1f && myState != states.TRACKING && myState != states.JUMP)
				{
					StopCoroutine("attacking");
					if (jumpStack == maxjumpstack)
					{
						myState = states.JUMP;
						prefromAnim(JUMP);
						jumpStack = 0;
						isAtk = true;
						isJump = true;
					}
					else
					{
						myState = states.TRACKING;
					}


				}

				if (myState == states.TRACKING)
				{
					agent.SetDestination(enemyAI.target.transform.position);
				}
				else if (myState == states.ATK && !isJump)
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
					isJump = false;
				}

			}

			if (myState != states.ATK && agent.velocity != Vector3.zero && myState != states.JUMP)
			{
				anim.SetInteger("Speed", RUN);
			}
			else if (myState != states.ATK && myState != states.JUMP)
			{
				anim.SetInteger("Speed", IDLE);
			}

		}
		else if (isOpen)
		{
		}
		else if(enemyAI.isDeath)
        {
            prefromAnim(DEATH);
        }

    }
    void RePositon()
    {
        if (enemyAI.isDeath)
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
        gameObject.GetComponent<Collider>().enabled = false;
        isDeath = true;
        StartCoroutine("attacking");
        myState = states.NONE;
        anim.SetInteger("Speed", 6);
    }

    public void endAtk()
    {
        isAtk = false;
       
    }

    public void activeAtk(atkTag tag)
    {
        switch(tag)
        {
            case atkTag.FOOT:
                footAtk.particle.Play();
                footAtk.hitbox.activeHitbox();
                break;
            case atkTag.JUMP:
                jumpAtk.particle.Play();
                jumpAtk.hitbox.activeHitbox();
                break;
        }
    }
    public void hideAtk(atkTag tag)
    {
        switch (tag)
        {
            case atkTag.FOOT:
                footAtk.hitbox.deActiveHitbox();
                break;
            case atkTag.JUMP:
                jumpAtk.hitbox.deActiveHitbox();
               
                break;
        }
    }

    public void showAlert(atkTag tag)
    {
        switch (tag)
        {
            case atkTag.FOOT:
                footAtk.alertArea.SetActive(true);
                break;
            case atkTag.JUMP:
                jumpAtk.alertArea.SetActive(true);
                break;
        }
    }
    public void hideAlert(atkTag tag)
    {
        switch (tag)
        {
            case atkTag.FOOT:
                footAtk.alertArea.SetActive(false);
                break;
            case atkTag.JUMP:
                jumpAtk.alertArea.SetActive(false);
                break;
        }
    }


    public void prefromAnim(int i)
    {
        anim.SetInteger("Speed", i);
    }
    public void prefromNormalAtk()
    {
        if (enemyAI.target != null)
        {
            if (Vector3.Distance(transform.position, enemyAI.target.transform.position) < enemyAI.stopRange + 0.5f)
            {
                if (jumpStack < maxjumpstack) jumpStack++;
                unitHitbox unitHB = enemyAI.target.GetComponent<unitHitbox>();
                unitHB.takenDamage(normalAtk.hitbox.damage,normalAtk.hitbox.ownerUnit);
            }
        }
    }
    public void endOpen()
    {
        isOpen = false;
    }
    public void selfDestruction()
    {
        Destroy(gameObject);
    }

    public void takeOff()
    {
        StartCoroutine(jumping());
    }

    IEnumerator jumping()
    {
        while(isJump)
        {
            yield return new WaitForFixedUpdate();
            Debug.Log("jumping");
            transform.Translate(Vector3.forward * jumpSpeed * Time.deltaTime);
        }
    }
    public void landing()
    {
        StopCoroutine("jumping");
        
        isJump = false;
        myState = states.TRACKING;

        agent.SetDestination(transform.position);

    }
    public void playMechSkillSound(AudioClip clip)
    {
        skillAudio.PlayOneShot(clip);
    }

}
