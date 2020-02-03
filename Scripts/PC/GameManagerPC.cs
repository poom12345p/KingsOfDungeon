using System.Collections.Generic;
using UnityEngine;

public class GameManagerPC : MonoBehaviour
{

    // singleton
    public static GameManagerPC Instance { get; private set; }
    public PlayerManagement playerManagement;

    [Header("Object that can activate on anyScene")]
    public EscapeMenu escapeMenu;
    public ShowGameState gameState;
    public ShowScore showScore;
    public LoadingScreenControl loadingScreenControl;
    public GameAssetStorage gameAssetStorage;

    // UI
    [Header("Menu Management")]
    public UIManager mainUI;
    public string lobbyRoomName;

    [Header("Sound")] //Sound
    public SoundManager soundManager;

    [Header("GamePlay")]
    public string gameSession = "";

    public PlayerInfo playerTutorial;


    [Header("Scene Management")] // SCENE
    const string MainMenuScene = "MainMenu";
    const string KingStateScene = "KingState";
    public int mapLevel = 0;
    public string[] allMap;

    // GAME STATE
    public enum GameState
    {
        Menu, Lobby, Tutorial, GamePlay, TutorialPlay
    }
    public GameState state = GameState.Menu;

    public SaveManager saveManager;

    void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerManagement = new PlayerManagement();
        // loadingScreenControl = LoadingScreenControl.Instance;
    }

    void Start()
    {
        mapLevel = 0;
        gameSession = "";
        saveManager = GetComponent<SaveManager>();
        soundManager = GetComponent<SoundManager>();
        ResetAllAssociateObject();
    }

    // SOMEONE CONNECTED TO SERVER
    public void OnPlayerConnectedToGame(PlayerInfo player)
    {
        int playerIndex = playerManagement.PlayerConnected(player);
        if (state == GameState.Lobby)
        {
            if (mainUI != null) mainUI.standbyMulti[playerIndex].ShowPlayer(player);
            if (playerManagement.IsMaximumPlayer()) EndLobby();
        }
        else if (state == GameState.Tutorial)
        {
            playerTutorial = player;
            if (mainUI != null) mainUI.standbyTut.ShowPlayer(player);
            EndLobby();
        }

    }

    // Player Disconnected
    public void OnPlayerDisconnected(int connectId)
    {
        int index = playerManagement.ManagePlayerDisconnected(connectId);
        if (state == GameState.Lobby)
        {
            if (mainUI != null) mainUI.standbyMulti[index].HidePlayer();
            if (MyServer.Instance != null && !MyServer.Instance.IsBroadCast())
                StartLobby(mainUI.roomName.text);
        }
        else if (state == GameState.Tutorial)
        {
            if (connectId == playerTutorial.connectionChannel)
            {
                if (mainUI != null) mainUI.standbyTut.HidePlayer();
                StartTutorialLobby();
            }
        }
    }

    public void PlayerPressReady(bool ready, int playerIndex)
    {
        if (mainUI != null) mainUI.PlayerReady(ready, playerIndex);
    }

    public void StartLobby(string roomName)
    {
        lobbyRoomName = roomName;
        if (MyServer.Instance != null && !MyServer.Instance.IsBroadCast())
            MyServer.Instance.StartLobbyBroadcasting(roomName);
    }

    public void StartTutorialLobby()
    {
        if (MyServer.Instance != null && !MyServer.Instance.IsBroadCast())
            MyServer.Instance.StartLobbyBroadcasting("Tutorial");
    }

    public void EndLobby()
    {
        if (MyServer.Instance != null && MyServer.Instance.IsBroadCast())
            MyServer.Instance.EndLobbyBroardcasting();
    }

    // Manage Player
    public void OnPlayerDeath(int id)
    {
        PlayerInfo player = playerManagement.GetPlayerByID(id);
        if (player != null && !player.isDead)
        {
            player.isDead = true;
            MyServer.Instance.SendPlayerStatus("DEAD", player);
            // Debug.Log(player + "is dead");
        }

        // Check if all player have died
        int deadPlayer = playerManagement.CountDeadPlayer();
        if (deadPlayer == playerManagement.connectedPlayer)
        {
            // Debug.Log("........Games End......");

            // save data when dead
            saveManager.SavePlayerInfoInstance(playerManagement.playersInfo, gameSession);


            escapeMenu.gameObject.SetActive(false);
            showScore.gameObject.SetActive(true);
            showScore.ShowScoresBoard();
        }
    }
    public void OnPlayerRevive(int id)
    {

        PlayerInfo player = playerManagement.GetPlayerByID(id);
        if (player != null && player.isDead)
        {
            player.isDead = false;
            MyServer.Instance.SendPlayerStatus("REVIVE", player);
            // Debug.Log(player + "is revived");
        }
    }
    public void OnPlayerRecieveDamage(int id)
    {

        PlayerInfo player = playerManagement.GetPlayerByID(id);
        if (player != null && !player.isDead && !player.isDisconnected)
        {
            MyServer.Instance.SendPlayerStatus("ATTACKED", player);
        }
    }

    public void sendMsgToController(int id, string msg)
    {
        PlayerInfo player = playerManagement.GetPlayerByID(id);
        if (player != null)
        {
            MyServer.Instance.SendPlayerStatus(msg, player);
            // Debug.Log(player + "Send :" + msg);
        }

    }
    // Scene Loading
    public void LoadGamePlay()
    {
        EndLobby();
        state = GameState.GamePlay;
        // generate session string
        gameSession = GenerateUid();
        MyServer.Instance.SendChangeStateMessage("MultiPlay");
        loadingScreenControl.gameObject.SetActive(true);
        LoadNextRandomMap();
    }

    public void AfterFinishLoading()
    {
        if (state == GameState.GamePlay && mapLevel < 2)
        {
            gameState.gameObject.SetActive(true);
            escapeMenu.gameObject.SetActive(true);
        }
    }

    public void LoadNextState()
    {
        bool playerExists = saveManager.playerRank.PlayerExistsInState(mapLevel);
        // Debug.Log("Go Next State TeamExists: " + playerExists);

        if (playerExists)
        {
            gameState.KingStateShow();
            loadingScreenControl.ShowLoading(KingStateScene);
        }
        else
        {
            LoadNextRandomMap();
        }
    }

    public void LoadNextRandomMap()
    {
        mapLevel++;
        gameState.GameStateChange(mapLevel);
        Random.InitState((int)Time.realtimeSinceStartup);
        string map = allMap[Random.Range(0, allMap.Length)];
        LoadMap(map);
    }

    public void LoadMap(string mapName)
    {
        for (int i = 0; i < playerManagement.playersInfo.Length; i++)
        {
            if (playerManagement.playersInfo[i] != null)
                playerManagement.AddMapScore(i, mapLevel - 1);
        }

        if (loadingScreenControl != null)
            loadingScreenControl.ShowLoading(mapName);
        else Debug.LogError("No loadingScreenControl instance");
    }

    public void LoadTutorial(PlayerInfo player)
    {
        playerTutorial = player;
        state = GameState.TutorialPlay;
        EndLobby();

        // generate session string
        gameSession = GenerateUid();

        MyServer.Instance.SendChangeStateMessage("Tutorial");
        loadingScreenControl.ShowLoading("TutorialScene");

        escapeMenu.gameObject.SetActive(true);
    }

    public void LoadMenu()
    {
        loadingScreenControl.ShowLoading(MainMenuScene);
    }

    public void ResetValueAndGoToMenu()
    {
        playerTutorial = null;
        ResetAllAssociateObject();

        mapLevel = 0;
        gameSession = "";

        int countPlayer = 0;
        for (int i = 0; i < PlayerManagement.maxPlayer; i++)
        {
            PlayerInfo player = playerManagement.playersInfo[i];
            if (player != null)
            {
                // game end but still disconnect ? just remove it
                if (player.isDisconnected)
                {
                    playerManagement.playersInfo[i] = null;
                }
                else
                {
                    player.isDead = false;
                    player.resetScore();
                    ++countPlayer;
                }
            }
        }

        playerManagement.connectedPlayer = countPlayer;

        // TELL CLIENT TO GO BACK
        if (MyServer.Instance != null)
        {
            MyServer.Instance.SendChangeStateMessage("GameEnd");
            MyServer.Instance.ClearBufferedMessage();
        }


        ClearAllPlayerInstance();
        LoadMenu();
    }

    public void addPlayerDontDestroyOnLoad(int index, GameObject player)
    {
        ItemOnPlayer itemOnPlayer = player.AddComponent<ItemOnPlayer>() as ItemOnPlayer;
        itemOnPlayer.SetItemHolder(playerManagement.playersInfo[index]);
        playerManagement.playerInstance[index] = player;
        DontDestroyOnLoad(playerManagement.playerInstance[index]);
    }

    public void ClearAllPlayerInstance()
    {
        // destroy all player that has been set dont destroy on load
        for (int i = 0; i < PlayerManagement.maxPlayer; i++)
        {
            if (playerManagement.playerInstance[i] != null)
                Destroy(playerManagement.playerInstance[i]);
        }
    }
    public void RequestUsingItem(int itemCode, int connectId)
    {

        // for (int i = 0; i < playerManagement.playersInfo.Length; i++)
        // {
        //     PlayerInfo player = playerManagement.playersInfo[i];
        //     if (player != null && player.connectionChannel == connectId)
        //     {
        //         Debug.LogWarning("up skill for player " + player);
        //     }
        // }
    }

    void ResetAllAssociateObject()
    {
        if (showScore != null) showScore.ResetValue();
        gameState.gameObject.SetActive(false);
        escapeMenu.ShowHideEscapeMenuList(false);
        escapeMenu.gameObject.SetActive(false);
        loadingScreenControl.Hide();
    }

    public string GenerateUid()
    {
        return System.DateTime.Now.ToFileTime() +
        ":" +
        Random.Range(0, 1000000) +
        ":" + Random.Range(0, 1000000) +
        lobbyRoomName.GetHashCode();
    }

    public void TriggerWhenStateClear()
    {
        gameState.TriggerHintClearState();
    }

}
