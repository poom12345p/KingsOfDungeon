using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameManagerMobile : MonoBehaviour
{

    // singleton
    public static GameManagerMobile Instance { get; private set; }

    // main player info
    public PlayerInfo playerInfo;
    public bool ready = false;

    public GameObject deadPanel;
    bool isDead = false;

    // ui
    public GameObject[] disconnectPage;
    public UIMobile uiManager;

    public string gameSession = "";
    public int playerNumber = -1;
    public string myJob;

    public InventorySystem inventorySystem;
    public CoinSystem coinSystem;

    // GAME STATE
    public enum GameState
    {
        Menu, Controller, TutorialController
    }
    public GameState state = GameState.Menu;

    public bool startGame = true;

    public SoundManager soundManager;

    public GameAssetStorage gameAsset;

    public Controller myController;

    // [Header("PlayerStat")]
    StatsShowMobile statsShow;
    PlayerStatMobile currentStat;

    public LoadingScreenControl loadingScreenControl;


    [Header("Server Loading Manage")]
    [SerializeField]
    private bool serverIsLoading;

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
    }

    void Start()
    {
        string currentUid = PlayerPrefs.GetString("Uid");

        Debug.Log(currentUid);

        if (currentUid == "")
        {
            currentUid = GenerateUniqueId();
            PlayerPrefs.SetString("Uid", currentUid);
        }

        playerInfo = new PlayerInfo(
            currentUid,
            PlayerPrefs.GetString("Name"),
            PlayerPrefs.GetInt("Level"),
            myJob
        );

        if (myJob != "") playerInfo.job = myJob;
        if (deadPanel != null) deadPanel.SetActive(false);

        loadingScreenControl.Hide();
        HideDisconnectedPage();
    }

    public void HideDisconnectedPage()
    {
        for (int i = 0; i < disconnectPage.Length; i++)
            disconnectPage[i].SetActive(false);
    }

    public void ReConnectToServer()
    {
        disconnectPage[0].SetActive(false);
        disconnectPage[1].SetActive(true);
        MyClient.Instance.Connect();
        StartCoroutine("DelayConencted");

    }

    IEnumerator DelayConencted()
    {
        yield return new WaitForSeconds(4f);

        if (!MyClient.Instance.IsConnectedToServer())
        {
            disconnectPage[0].SetActive(true);
            disconnectPage[1].SetActive(false);
        }
    }

    public void ConnectedSuccessfull()
    {
        if (state == GameState.Menu)
        {
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIMobile>();
                if (uiManager == null)
                {
                    // still can't find 
                    Debug.LogError("No instance of UI Mobile");
                }
            }
            else
            {
                uiManager.ConnectedSuccessfull();
            }

        }
        else
        {
            Debug.Log("Reconnected in controller scene");
        }

        HideDisconnectedPage();
    }

    public void AddLanConnection(List<LanConnectionInfo> lan)
    {
        uiManager.GenerateConnectionButton(lan);
    }

    public void StartQuickPlay()
    {
        state = GameState.Controller;
        LoadingScreenControl.Instance.ShowLoading("Controller");


    }

    public void StartTutorial()
    {
        state = GameState.TutorialController;
        LoadingScreenControl.Instance.ShowLoading("Controller");

    }

    public void BackToMeu()
    {
        state = GameState.Menu;
        ResetVariable();
        LoadingScreenControl.Instance.ShowLoading("Menu");
    }

    public void DisconnectedFromServer()
    {
        disconnectPage[0].SetActive(true);
        disconnectPage[2].SetActive(true);
    }

    public void PlayerIsDead(bool dead)
    {
        if (deadPanel != null) deadPanel.SetActive(dead);
        isDead = dead;
    }

    public void ResetVariable()
    {
        ready = false;
        if (deadPanel) deadPanel.SetActive(false);
        gameSession = "";
        HideDisconnectedPage();
    }

    public void DisconnectedSelf()
    {
        if (MyClient.Instance.IsConnectedToServer())
        {
            MyClient.Instance.DisconnectServer();
            DisconnectedFromServer();
        }
    }

    public void ChangeConecction()
    {
        if (state == GameState.Controller)
        {
            BackToMeu();
        }

        uiManager.GetNewRoom();
    }

    public void StoreItemInInventory(int itemCode)
    {

        if (itemCode == 0)
        {
            if (coinSystem == null) Debug.LogWarning("No coin system instance...");
            else coinSystem.UpdateCoin(1);
        }
        else
        {
            inventorySystem.AddItem(itemCode);
        }
    }

    public void RemoveItemInInventory(int itemCode)
    {
        print("remove item " + itemCode);
        inventorySystem.RemoveItem(itemCode);
    }

    public void editJob(string job)
    {
        playerInfo.job = job;
        myJob = job;
    }

    public void setStateShow(StatsShowMobile statsShow)
    {
        this.statsShow = statsShow;
        // TODO: reload all stat to stat show
        this.statsShow.OnStatChange(currentStat);
    }

    public void PlayerStatChange(PlayerStatMobile stat)
    {
        if (statsShow != null)
        {
            statsShow.OnStatChange(stat);
        }

        currentStat = stat;

    }

    public string GenerateUniqueId()
    {
        // TODO: generate unique identifier 
        return System.DateTime.Now.ToFileTime() + ":" + Random.Range(0, 1000000) + ":" + Random.Range(0, 1000000);
    }

    public void OnLoadingStateOfServerChange(bool serverLoading)
    {
        serverIsLoading = serverLoading;
        // TODO: Handle Loading state
    }
}
