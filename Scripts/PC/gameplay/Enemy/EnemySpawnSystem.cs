using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{

    public static EnemySpawnSystem Instance { get; private set; }
    public particleControl defalseSpawnParticle;
    // public List<EnemyAI> allEnemy;

    public GameObject[] enemy;
    public GameObject[] boss;

    void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject SpawnEnemy(GameObject enemy, Vector3 spawnPlace)
    {
        GameObject spawnEnemy = Instantiate(enemy, spawnPlace, Quaternion.identity);

        EnemyAI eAI = spawnEnemy.GetComponent<EnemyAI>();
        // allEnemy.Add(eAI);

        return spawnEnemy;
    }

    public GameObject GetRandomEnemy()
    {
        return enemy[Random.Range(0, enemy.Length)];
    }

    public GameObject GetRandomBoss()
    {
        return boss[Random.Range(0, boss.Length)];
    }

}
