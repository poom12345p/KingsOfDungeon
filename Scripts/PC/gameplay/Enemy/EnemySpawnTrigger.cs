using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{

    [System.Serializable]
    public struct SpecificEnemy
    {
        public GameObject enemy;
        public int spawnAmount;
        public Transform spawnPosition;
        public int increaseAmountPerPlayer;
    }

    public List<SpecificEnemy> specificEnemy;
    private particleControl defalseSpawnParticle;

    // Use this for initialization
    private EnemySpawnSystem enemySpawnSystem;
    public List<GameObject> enemySpawnedHere;
    public bool alreadySpawned;
    private Collider col;

    public int spawnedRandomEnemyAmount = 3;
    public int spawnMorePerPlayer = 4;
    const float defaultDelay = 2f;



    void Start()
    {

        enemySpawnSystem = EnemySpawnSystem.Instance;
        if (enemySpawnSystem == null) throw new System.Exception("no enemySpawnSystem in scene");

        col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
        defalseSpawnParticle = enemySpawnSystem.defalseSpawnParticle;
        defalseSpawnParticle.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !alreadySpawned)
        {
            StartSpawn();
        }
    }

    IEnumerator SpawnParticle(GameObject enemy, Vector3 SP)
    {
		//spawn particle 
		//float delay = enemy.GetComponent<EnemyAI>().spawnParticleDelay;
		ParticleSystem temp = new ParticleSystem();
		if (enemy.tag != "Untagged")
		{
			particleControl p = enemy.GetComponent<EnemyAI>().spawnParticle;
			if (p != null)
			{
				if (!p.gameObject.activeInHierarchy)
					p.gameObject.SetActive(true);
				Instantiate(p, SP, Quaternion.identity);
			}
			else
			{
				temp = Instantiate(defalseSpawnParticle.particle, SP, Quaternion.identity);
			}
		}
		//yield return new WaitForSeconds((delay == 0 ? defaultDelay : delay));
		yield return new WaitForSeconds(defaultDelay);

		//spawn monster
		enemy.gameObject.SetActive(true);
		if (temp != null)
		{
		Destroy(temp.gameObject);
		}
    }

    public void StartSpawn()
    {
        int playerAmount = GameManagerPC.Instance == null ? 0 : GameManagerPC.Instance.playerManagement.connectedPlayer - 1;

        if (specificEnemy.Count == 0)
        {
            for (int i = 0; i < spawnedRandomEnemyAmount + (spawnMorePerPlayer * playerAmount); i++)
            {
                GameObject enemy = enemySpawnSystem.GetRandomEnemy();
                Vector3 p = RandomSpawnPlace();

                GameObject e = enemySpawnSystem.SpawnEnemy(
                    enemy,
                    p
                );
                enemySpawnedHere.Add(e);
				e.SetActive(false);

				StartCoroutine(SpawnParticle(e, p));

                //GameObject e = enemySpawnSystem.SpawnEnemy(
                //	enemySpawnSystem.GetRandomEnemy(),
                //	RandomSpawnPlace()
                //);
                //enemySpawnedHere.Add(e);
                //if(!e.gameObject.activeInHierarchy) e.gameObject.SetActive(true);

                //if(GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
                //	e.GetComponent<StatContol>().SetAttackDamageToOne();
            }
        }
        else
        {
            foreach (var se in specificEnemy)
            {
                for (int i = 0; i < se.spawnAmount + (se.increaseAmountPerPlayer * playerAmount); i++)
                {
                    GameObject enemy = se.enemy;
                    Vector3 p = (se.spawnPosition == null ? Vector3.zero : se.spawnPosition.position);

                    GameObject e = enemySpawnSystem.SpawnEnemy(
                        enemy,
                        p
                    );
                    enemySpawnedHere.Add(e);
					e.SetActive(false);
					StartCoroutine(SpawnParticle(e, p));


                    //GameObject e = enemySpawnSystem.SpawnEnemy(
                    //	se.enemy,
                    //	(se.spawnPosition == null ? Vector3.zero : se.spawnPosition.position) 
                    //);
                    //enemySpawnedHere.Add(e);
                    e.transform.rotation = se.spawnPosition.rotation;
                    // if (!e.gameObject.activeInHierarchy) e.gameObject.SetActive(true);
                }
            }
        }



        alreadySpawned = true;

    }

    private Vector3 RandomSpawnPlace()
    {
        Vector3 colBound = col.bounds.extents;
        Vector3 pos = transform.position;
        return new Vector3(
            pos.x + Random.Range(-colBound.x, colBound.x),
            pos.y + colBound.y,
            pos.z + Random.Range(-colBound.z, colBound.z)
        );
    }

    public void SetCollider(Collider c)
    {
        col = c;
    }

}
