using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class PlayersCommander : MonoBehaviour
{

    public Transform spawnPoint;
    public GameObject[] characters;
    public GameObject[] players = new GameObject[4];
    public CinemachineTargetGroup TargetGroup;
    public nameTagControl nameTagControl;
    public PointerControl pointerControl;
    public int playersRadius;
    [SerializeField] CharecterControl[] playersCons = new CharecterControl[4];
    [System.Serializable]
    public struct charactersMatch
    {
        public string charClass;
        public GameObject charPrefab;
    }
    public charactersMatch[] characters2;
    // Use this for initialization
    void Start()
    {
        if (MyServer.Instance != null) MyServer.Instance.GetPlayerComander(this);
        if (GameManagerPC.Instance != null) createCharector();
    }

    public void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (nameTagControl != null)
                {
                    nameTagControl.setNameTagToPlayer(i, players[i].transform);
                    if (pointerControl != null)
                        pointerControl.checkPosition(i, players[i].transform);
                    else
                        Debug.LogWarning("No pointer Control in this playerCommander instance, might be a problem later...");
                }
            }
        }
    }

    public void setPlayer(int i, string ch)
    {
        // Debug.Log("index: " + i + ":" + ch);
        if (GameManagerPC.Instance.playerManagement.playerInstance[i] == null)
        {
            players[i] = Instantiate(getCharecterPrefab(ch), spawnPoint.position, Quaternion.identity);
            GameManagerPC.Instance.addPlayerDontDestroyOnLoad(i, players[i]);
        }
        else
        {
            // already spwn player
            // just set player position 
            players[i] = GameManagerPC.Instance.playerManagement.playerInstance[i];
            players[i].GetComponent<NavMeshAgent>().Warp(spawnPoint.transform.position);
        }

        playersCons[i] = players[i].GetComponent<CharecterControl>();
        players[i].GetComponent<unitHitbox>().setID(i);
        playersCons[i].setID(i);
        // var target = new CinemachineTargetGroup.Target { target = players[i].transform, radius = 0, weight =1 };
        TargetGroup.m_Targets[i].target = players[i].transform;
        TargetGroup.m_Targets[i].radius = playersRadius;
        if (nameTagControl != null) nameTagControl.create(i, GameManagerPC.Instance.playerManagement.playersInfo[i].playerName);

    }

    GameObject getCharecterPrefab(string ch)
    {
        int i = -1;
        // print("character " + ch);

        if (GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
        {
            TutorialManager.Instance.SetTutorialFor(ch);
        }

        if (GameManagerPC.Instance.gameAssetStorage != null)
        {
            return GameManagerPC.Instance.gameAssetStorage.FindCharacterByTitle(ch);
        }

        switch (ch)
        {
            case "KNIGHT":
                i = 0;
                break;
            case "ARCHER":
                i = 1;
                break;
            case "WIZARD":
                i = 2;
                break;
            case "LANCER":
                i = 3;
                break;
            default:
                return null;
        }

        return characters[i];
    }



    public void sendComand(int id, string cmd)
    {
        // print(id);
        playersCons[id].getCommand(cmd);
    }
    public void sendUpSkill(int id, int num)
    {
        //        print(id);
        playersCons[id].getUpSkill(num);
    }
    public void createCharector()
    {
        if (GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
        {
            PlayerInfo player = GameManagerPC.Instance.playerTutorial;
            setPlayer(player.playerId, player.job);
        }
        else
        {

            for (int i = 0; i < 4; i++)
            {
                PlayerInfo PNF = GameManagerPC.Instance.playerManagement.playersInfo[i];
                if (PNF != null)
                {
                    setPlayer(PNF.playerId, PNF.job);
                }
            }

        }


    }
}
