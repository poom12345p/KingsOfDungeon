using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMobile : MonoBehaviour
{

    // Menu
    [Header("Menu")]
    public GameObject[] canvas; // menu RenameField connect
    int currentCanvas;
    public StanbyCharacterManagerMobile standby;
    public CharStatUI charStatUI;

    public Text readyText;
    public Image playerSign;
    public Sprite[] signSprite;

    //INTRO COMPONENTS
    [Header("Intro")]
    public GameObject newPlayer;
    public InputField nameInput;
    public Text ErrorNamePlayerInput;

    //PLAYERPREFS
    [Header("Player")]
    public Text playerNameTexts;
    public InputField playerNameInput;
    public GameObject editButton;
    public GameObject confirmPlayerInputObject;
    EventSystem m_eventSystem;
    public Text level;

    //CONNECTING
    [Header("Connection")]
    public Text connectionStatus;
    public GameObject connectCanvas;
    public GameObject block;

    // Button To connect
    public Button connect;
    public ItemsScrollList scrollList;

    [Header("ButtonSound")]
    public AudioClip[] buttonClickClip;

    [Header("PopUp")]
    public GameObject ipRoomEnterPopUp;
    public RenameUI renameUI;

    void Start()
    {

        if (PlayerPrefs.GetString("Name") != "" && playerNameTexts != null)
            ChangeUserNameText(PlayerPrefs.GetString("Name"));
        if (PlayerPrefs.GetInt("Level") != 0 && level != null)
            level.text = PlayerPrefs.GetInt("Level").ToString();

        if (connectionStatus != null)
            connectionStatus.text = "";

        currentCanvas = 0;

        if (ErrorNamePlayerInput != null)
            ErrorNamePlayerInput.gameObject.SetActive(false);

        GameManagerMobile.Instance.uiManager = this;
        if (readyText != null) readyText.color = Color.white;

        if (!GameManagerMobile.Instance.startGame)
        {
            newPlayer.transform.parent.gameObject.SetActive(false);
            ShowMain();
        }

        m_eventSystem = EventSystem.current;

        Debug.Log(GameManagerMobile.Instance.GenerateUniqueId());
    }

    public void TapStart()
    {
        if (PlayerPrefs.GetString("Name") != "")
        {
            newPlayer.transform.parent.gameObject.SetActive(false);
            connectCanvas.SetActive(true);
            MyClient.Instance.ListenForRoom();
            block.SetActive(false);
        }
        else
        {
            newPlayer.SetActive(true);
        }

        GameManagerMobile.Instance.startGame = false;
    }

    public void RegisterNewPlayer()
    {
        if (nameInput.text != "" && nameInput.text.Length <= 12)
        {
            string uid = GameManagerMobile.Instance.GenerateUniqueId();
            PlayerPrefs.SetString("Name", nameInput.text);
            PlayerPrefs.SetInt("EXP", 0);
            PlayerPrefs.SetInt("Level", 1);

            ChangeUserNameText(PlayerPrefs.GetString("Name"));
            level.text = "1";

            GameManagerMobile.Instance.playerInfo.playerName = nameInput.text;
            GameManagerMobile.Instance.playerInfo.level = 1;

            newPlayer.transform.parent.gameObject.SetActive(false);
            connectCanvas.SetActive(true);
            if (block != null) block.SetActive(false);
            MyClient.Instance.ListenForRoom();
        }
        else
        {
            ErrorNamePlayerInput.gameObject.SetActive(true);
            if (nameInput.text == "")
            {
                ErrorNamePlayerInput.text = "Your name can't be blank!";
            }
            else
            {
                ErrorNamePlayerInput.text = "Your name can't be longer than 12 character";
            }
        }
    }

    public void TapToConnect(string attachIp, int port = 25000)
    {
        if (attachIp != "")
        {
            SoundManager sm = GameManagerMobile.Instance.soundManager;
            sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
            MyClient.Instance.Connect(attachIp, port);
            if (connectionStatus != null) connectionStatus.text = "connecting...";
            if (block != null) block.SetActive(true);
            StartCoroutine("DelayConnected");
        }
    }

    public void TapToConnect(InputField textinput)
    {
        if (textinput.text != "")
        {
            MyClient.Instance.Connect(textinput.text, 25000);
            if (connectionStatus != null) connectionStatus.text = "connecting...";
            if (block != null) block.SetActive(true);
            StartCoroutine("DelayConnected");
        }
    }

    public void TestToController()
    {
        GameManagerMobile.Instance.StartQuickPlay();
    }

    public void GetNewRoom()
    {
        if (!GameManagerMobile.Instance.ready)
        {
            connectionStatus.text = "";
            MyClient.Instance.DisconnectServer();
            ChangeCanvas(2);
            MyClient.Instance.ListenForRoom();
        }
    }

    public void PressedReady(GameObject rdBtn)
    {
        SoundManager sm = GameManagerMobile.Instance.soundManager;
        sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
        if (GameManagerMobile.Instance.playerInfo.job != "")
        {
            GameManagerMobile.Instance.ready = !GameManagerMobile.Instance.ready;
            MyClient.SendMessageReady();

            if (GameManagerMobile.Instance.ready)
            {
                rdBtn.SetActive(true);
            }
            else
            {
                rdBtn.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Choose your job first");
            ChangeCharacterJob("KNIGHT");
        }
    }

    public void ConnectedSuccessfull()
    {
        // if connect successful
        if (MyClient.Instance.IsConnectedToServer())
        {
            block.SetActive(false);
            StopAllCoroutines();
            MyClient.Instance.StopListen();
            ShowMain();
        }

    }

    public void GenerateConnectionButton(List<LanConnectionInfo> AllLan)
    {
        scrollList.RemoveAll();
        foreach (LanConnectionInfo lan in AllLan)
        {

            string ip = lan.ip;
            int port = lan.port;
            string roomName = lan.roomName;
            string btnName = lan.roomName + ":" + lan.port;

            if (scrollList.inactiveInstances.Count > 0)
            {
                // recycle use
                GameObject btn = scrollList.RecycleScrollList();
                btn.SetActive(true);
                btn.GetComponentInChildren<Text>().text = roomName;
                btn.name = btnName;
                btn.GetComponent<Button>().onClick.AddListener(delegate
                {
                    SoundManager sm = GameManagerMobile.Instance.soundManager;
                    sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
                    TapToConnect(ip, port);
                });

                scrollList.AddItem(btn);
            }
            else if (scrollList.inactiveInstances.Count == 0)
            {
                // create new one
                Button newBtn = Instantiate(connect);
                newBtn.GetComponentInChildren<Text>().text = roomName;
                newBtn.gameObject.name = btnName;
                newBtn.onClick.AddListener(delegate
                {
                    SoundManager sm = GameManagerMobile.Instance.soundManager;
                    sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
                    TapToConnect(ip, port);
                });

                scrollList.AddItem(newBtn.gameObject);
            }
        }
        scrollList.ShowList();
    }


    IEnumerator DelayConnected()
    {
        yield return new WaitForSeconds(4f);

        if (!MyClient.Instance.IsConnectedToServer())
        {
            block.SetActive(false);
            connectionStatus.text = "Error, Please Try Again";
        }
    }

    public void ChangeCanvas(int index)
    {
        if (!GameManagerMobile.Instance.ready)
        {
            SoundManager sm = GameManagerMobile.Instance.soundManager;
            sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
            canvas[currentCanvas].SetActive(false);
            currentCanvas = index;
            canvas[currentCanvas].SetActive(true);
        }

        standby.gameObject.SetActive(index == 0);
    }

    public void ChangeCharacterJob(string job)
    {
        SoundManager sm = GameManagerMobile.Instance.soundManager;
        sm.PlaySFXOneShot(buttonClickClip[Random.Range(0, 1)]);
        standby.gameObject.SetActive(true);
        standby.changeLook(job);
        charStatUI.setUI(job);
        GameManagerMobile.Instance.editJob(job);
        MyClient.SendPlayerInfo();

    }

    public void ShowMain()
    {
        connectCanvas.SetActive(false);
        canvas[0].SetActive(true);
        currentCanvas = 0;
        standby.changeLook(GameManagerMobile.Instance.myJob);
        charStatUI.setUI(GameManagerMobile.Instance.myJob);
        playerSign.sprite = signSprite[GameManagerMobile.Instance.playerNumber];
    }

    public void openRenameUI()
    {
        renameUI.gameObject.SetActive(true);
    }

    public void ChangeUserNameText(string name)
    {
        playerNameTexts.text = name;
        playerNameInput.text = name;
    }

    public void Rename(string newName)
    {
        if (newName.Length >= 12)
        {
            Debug.LogError("Name can't be more than 12 character");
        }
        else if (newName.Length == 0)
        {
            Debug.LogError("Name can't be blank");
        }
        else
        {
            PlayerPrefs.SetString("Name", newName);
            ChangeUserNameText(newName);
            GameManagerMobile.Instance.playerInfo.playerName = newName;

            if (MyClient.Instance.IsConnectedToServer()) MyClient.SendPlayerInfo();
        }
    }

    public void Rename(Text nameText)
    {
        Rename(nameText.text);
    }

    public void TriggerPopUpEnterByIp()
    {

        ipRoomEnterPopUp.SetActive(!ipRoomEnterPopUp.activeInHierarchy);
    }

    public void TriggerNameInputEditable()
    {
        playerNameInput.interactable = !playerNameInput.interactable;
        confirmPlayerInputObject.SetActive(playerNameInput.interactable);
        editButton.SetActive(!playerNameInput.interactable);

        if (playerNameInput.interactable)
        {
            playerNameInput.OnPointerClick(new PointerEventData(m_eventSystem));
            m_eventSystem.SetSelectedGameObject(playerNameInput.gameObject, new BaseEventData(m_eventSystem));
        }

    }

    public void ConfirmEditName(Text nameText)
    {
        Rename(nameText);
        TriggerNameInputEditable();
    }
}
