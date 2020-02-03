using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandbyCharacterManager : MonoBehaviour
{

    private Dictionary<string, GameObject> playerLook; //knight archer berserker warchief

    // LOBBY VARIABLE
    public Text lobbyStatus;
    public Text lobbyPlayerName;

    // player info
    public PlayerInfo player;
    public bool isReady = false;

    public Image playerColorShow;

    void Awake()
    {

        playerLook = new Dictionary<string, GameObject>();

        foreach (Transform children in transform)
        {
            playerLook.Add(children.name, children.gameObject);
            children.gameObject.SetActive(false);
        }

    }

    // LOBBY CODE 
    public void ChangeStatusText(string textChange)
    {
        lobbyStatus.text = textChange;
    }

    public void ChangePlayerName(string name)
    {
        lobbyPlayerName.text = name;
    }

    public void ShowPlayer(PlayerInfo playerInfo)
    {
        //print("show player: "+playerInfo);
        ChangeStatusText("Waiting");
        lobbyPlayerName.gameObject.SetActive(true);
        ChangePlayerName(playerInfo.playerName);

        // Change character look
        if (playerLook.ContainsKey(playerInfo.job))
        {
            if (player != null)
                playerLook[player.job].SetActive(false);

            playerLook[playerInfo.job].SetActive(true);
            playerColorShow.gameObject.SetActive(true);
            player = playerInfo;
        }
    }

    public void HidePlayer()
    {
        ChangeStatusText("");
        lobbyStatus.color = Color.white;
        lobbyPlayerName.gameObject.SetActive(false);
        playerColorShow.gameObject.SetActive(false);

        if (player != null)
        {
            GameObject playerLookGameObject;
            playerLook.TryGetValue(player.job, out playerLookGameObject);
            if (playerLookGameObject != null) playerLookGameObject.SetActive(false);
            player = null;
        }
    }

    public void OnPlayerReady(bool ready)
    {
        isReady = ready;

        if (ready)
        {
            ChangeStatusText("Ready");
            lobbyStatus.color = Color.green;
        }
        else
        {
            ChangeStatusText("Waiting");
            lobbyStatus.color = Color.white;
        }
    }
}
