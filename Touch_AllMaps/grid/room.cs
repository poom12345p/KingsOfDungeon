using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class room : MonoBehaviour
{

    public UnityEvent afterAllSpawn;
    public UnityEvent afterFinisedRoom;

    public doorParticleControl dpc;
    public List<vertex> OpenedDoors;
    public vertex[] doors;
    public GameObject[] walls;
    public EnemySpawnSystem enemySpawnSystem;
    public RoomStatus status;

    [Header("Set Enemy Trigger")]
    public List<EnemySpawnTrigger> enemySpawnTrigger;

    [Space]
    public List<EnemyAI> allEnemyInRoom;
    public List<GameObject> allPlayerInRoom;
    public int maxEnemy;
    public int minEnemy;
    bool isPlayingInRoom = false;
    public int wave = 3;
    int waveCount = 1;
    bool CamIsMoving = false;

    // a parent randomSystem that it has been spawned from
    public randomSystem_03 randomFrom;

    private void Start()
    {
        enemySpawnSystem = EnemySpawnSystem.Instance;
        foreach (var trigger in enemySpawnTrigger)
        {
            trigger.SetCollider(GetComponent<Collider>());
        }
    }


    //--------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (status == RoomStatus.startRoom) return;

        if (other.tag == "Player")
        {

            allPlayerInRoom.Add(other.gameObject);

            if (!CamIsMoving && status != RoomStatus.clearedRoom && !isPlayingInRoom)
            {
                randomFrom.roomCam.m_LookAt.GetComponent<CinemachineTargetGroup>().m_Targets[0].target = other.transform;
                randomFrom.roomCam.Priority = 15;
                StartCoroutine(waitForCameraBlendFinishing(other.transform));
                //PlayerEnterRoom(other.transform);
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && allPlayerInRoom.Contains(other.gameObject))
        {
            allPlayerInRoom.Remove(other.gameObject);
        }
    }

    IEnumerator waitForCameraBlendFinishing(Transform playerEnter)
    {
        CamIsMoving = true;
        while (CinemachineCore.Instance.IsLive(randomFrom.mainCam))
        {
            yield return new WaitForFixedUpdate();
        }

        PlayerEnterRoom(playerEnter);
    }

    public void PlayerEnterRoom(Transform playerEnter)
    {
        if (GameManagerPC.Instance != null)
        {
            foreach (GameObject player in GameManagerPC.Instance.playerManagement.playerInstance)
            {
                if (player != null && player.transform != playerEnter && !allPlayerInRoom.Contains(player))
                {
                    player.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(playerEnter.position);
                }
            }
        }


        foreach (vertex door in OpenedDoors)
        {
            DoorControl dc = door.gameObject.GetComponent<DoorControl>();
            dc.CloseTheDoor();
        }

        try
        {
            isPlayingInRoom = true;
            RoomSetUp();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error Happend When setup Room " + gameObject.name + ": " + e.Message);
        }
        finally
        {
            CamIsMoving = false;
            randomFrom.roomCam.Priority = 5;
        }
        // Run Gameplay things

    }
    public void RoomSetUp()
    {
        if (status == RoomStatus.normalRoom)
        {
            // spawn enemy
            dpc.startparticle();
            int triggerRandom = Random.Range(0, enemySpawnTrigger.Count);
            SpawnRandomEnemy(triggerRandom);
        }
        else if (status == RoomStatus.bossRoom)
        {
            // spawn boss
            dpc.startparticle();
            SpawnBoss(enemySpawnSystem.GetRandomBoss());

            int triggerRandom = Random.Range(0, enemySpawnTrigger.Count);
            // spawn minion
            SpawnRandomEnemy(triggerRandom);
        }
        else if (status == RoomStatus.healRoom)
        {
            // special room
            FinishedPlayInRoom();
        }
    }
    public void SpawnRandomEnemy(int spawnTrigerIndex)
    {
        if (status == RoomStatus.clearedRoom) return;
        int randomEnemySpawn = Random.Range(minEnemy, maxEnemy + 1);
        enemySpawnTrigger[spawnTrigerIndex].spawnedRandomEnemyAmount = randomEnemySpawn;
        enemySpawnTrigger[spawnTrigerIndex].StartSpawn();

        foreach (var e in enemySpawnTrigger[spawnTrigerIndex].enemySpawnedHere)
        {
            if (e != null)
            {
                EnemyAI enemyAI = e.GetComponent<EnemyAI>();
                if (enemyAI != null && !enemyAI.notTruelyEnemy)
                {
                    allEnemyInRoom.Add(e.GetComponent<EnemyAI>());
                    enemyAI.roomOwner = this;
                }
            }
        }
        afterSpawnAllMonster();
    }

    public void SpawnBoss(GameObject boss)
    {

        if (status == RoomStatus.clearedRoom) return;
        Collider col = GetComponent<Collider>();
        // get a random pos in side a collider Box
        Vector3 colBound = col.bounds.extents;
        Vector3 pos = transform.position;
        Vector3 randomPos = new Vector3(
            pos.x + Random.Range(-colBound.x, colBound.x),
            pos.y + colBound.y,
            pos.z + Random.Range(-colBound.z, colBound.z)
        );

        GameObject spawnEnemy = enemySpawnSystem.SpawnEnemy(boss, randomPos);
        spawnEnemy.transform.parent = transform;

        EnemyAI eAI = spawnEnemy.GetComponent<EnemyAI>();

        if (eAI != null)
        {
            allEnemyInRoom.Add(spawnEnemy.GetComponent<EnemyAI>());
            eAI.roomOwner = this;
        }

        afterSpawnAllMonster();
    }

    public void SpawnReward(EnemySpawnTrigger triggerReward)
    {
        triggerReward.StartSpawn();
    }

    public void FinishedPlayInRoom()
    {
        foreach (vertex door in OpenedDoors)
        {
            DoorControl dc = door.gameObject.GetComponent<DoorControl>();
            dc.OpenTheDoor();
        }
        isPlayingInRoom = false;

        if (status == RoomStatus.bossRoom)
        {
            randomFrom.WrapGateToTheNextLevel.gameObject.transform.position = new Vector3(
                transform.position.x - 2.5f,
                transform.position.y,
                transform.position.z - 2.5f
            );

            randomFrom.WrapGateToTheNextLevel.gameObject.SetActive(true);
            GameManagerPC.Instance.TriggerWhenStateClear();
        }


        afterFinisedRoom.Invoke();
        status = RoomStatus.clearedRoom;
        allEnemyInRoom.Clear();
        // if(randomFrom.OnAllRoomClear()){
        //     randomFrom.WrapGateToTheNextLevel.gameObject.SetActive(true);
        //     randomFrom.WrapGateToTheNextLevel.gameObject.transform.position = transform.position;
        // }
    }

    public void afterSpawnAllMonster()
    {
        afterAllSpawn.Invoke();
    }


    public void EnemyInRoomDead()
    {
        foreach (EnemyAI e in allEnemyInRoom)
        {
            if (!e.isDeath) return;
        }
        //        print("Finish wave : " + waveCount);
        if (waveCount >= wave || status == RoomStatus.bossRoom)
        {
            FinishedPlayInRoom();

        }
        else
        {
            maxEnemy += 3;
            minEnemy += 2;
            waveCount++;

            RoomSetUp();
        }
    }

}
