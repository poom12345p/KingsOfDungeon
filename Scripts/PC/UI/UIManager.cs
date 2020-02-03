using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    public GameObject[] canvas; //Intro Start Setting Quit Lobby Tutorial
    public GameObject block;

    public int goTo = -1;

    int currentIndex = 0;

    // MOVING CAMERA
    public GameObject[] camPos; // intro quickplay multi
    int currentCamPos = 0;
    bool camMoving = false;

    // Lobby name
    public GameObject roomNameInput;

    // LOBBY VARIABLE
    public Text gameStateShow;
    public Text roomName;
    public StandbyCharacterManager[] standbyMulti;
    //public Text PlayButton;

    // TUTORIAL
    public StandbyCharacterManager standbyTut;

    public AudioClip[] buttonClickSound;

    public Text IpTextShow;

    private void Start()
    {
        // set up
        gameStateShow.text = "";
        camMoving = true;
        roomNameInput.SetActive(false);
        GameManagerPC.Instance.mainUI = this;

        foreach (var standBy in standbyMulti)
        {
            standBy.HidePlayer();
        }
        standbyTut.HidePlayer();


        if (GameManagerPC.Instance.state == GameManagerPC.GameState.GamePlay)
        {
            roomName.text = GameManagerPC.Instance.lobbyRoomName;
            SelectCanvas(4);
        }
        else if (GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
        {
            SelectCanvas(5);
        }
        else
        {
            GameManagerPC.Instance.state = GameManagerPC.GameState.Menu;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (camMoving)
        {
            CamMoveTo(currentCamPos);
        }

    }

    public void TapToStart()
    {
        GameManagerPC.Instance.soundManager.PlaySFXOneShot(buttonClickSound[Random.Range(0, buttonClickSound.Length)]);
        StartCoroutine(FadeOutAndIn(0, 1));
        currentIndex = 1;
        // GameManagerPC.Instance.SetStartGame(false);
    }

    public void SelectCanvas(int index)
    {
        GameManagerPC.Instance.soundManager.PlaySFXOneShot(buttonClickSound[Random.Range(0, buttonClickSound.Length)]);
        camMoving = true;
        StartCoroutine(FadeOutAndIn(currentIndex, index));
        currentIndex = index;
        roomNameInput.SetActive(false);

        if (index == 4)
        {
            // multi lobby
            gameStateShow.text = "Lobby";
            currentCamPos = 2;
            canvas[1].SetActive(false);
            GameManagerPC.Instance.state = GameManagerPC.GameState.Lobby;
            GameManagerPC.Instance.StartLobby(roomName.text);
            IpTextShow.text = "IP: " + MyServer.Instance.ipAddress;
            ShowAllPlayerInMultiLobby();
        }
        else if (index == 5)
        {
            // tutorial
            gameStateShow.text = "Tutorial";
            currentCamPos = 3;
            canvas[1].SetActive(false);
            GameManagerPC.Instance.state = GameManagerPC.GameState.Tutorial;
            GameManagerPC.Instance.StartTutorialLobby();
            IpTextShow.text = "IP: " + MyServer.Instance.ipAddress;
            PlayerInfo player = GameManagerPC.Instance.playerManagement.GetFirstConnectedPlayer();

            // if someone already connected
            if (player != null)
            {
                standbyTut.ShowPlayer(player);
            }
            else
            {
                standbyTut.HidePlayer();
            }
        }
        else if (index < 4)
        {
            // back to main menu
            gameStateShow.text = "";
            currentCamPos = 0;
            canvas[1].SetActive(true);

            // back from lobby
            if (GameManagerPC.Instance.state == GameManagerPC.GameState.Lobby ||
               GameManagerPC.Instance.state == GameManagerPC.GameState.Tutorial)
            {
                MyServer.Instance.DisconnectAllClient();
                GameManagerPC.Instance.EndLobby();
            }

            GameManagerPC.Instance.state = GameManagerPC.GameState.Menu;
        }
    }

    public void ShowInputRoomName()
    {
        GameManagerPC.Instance.soundManager.PlaySFXOneShot(buttonClickSound[Random.Range(0, buttonClickSound.Length)]);
        roomNameInput.SetActive(true);
    }

    public void GetRoomName()
    {
        InputField getRoomName = roomNameInput.GetComponentInChildren<InputField>();

        if (getRoomName.text != "" || getRoomName.text.Length > 12)
        {

            roomName.text = getRoomName.text;
            SelectCanvas(4);
        }
        else
        {
            Debug.LogError("Please Enter Your Room name, Not blank, Not more than 12 letter");
        }


    }

    IEnumerator FadeOutAndIn(int i, int j)
    {

        block.SetActive(true);

        while (canvas[i].GetComponent<CanvasGroup>().alpha > 0)
        {
            yield return new WaitForSeconds(0.005f);
            canvas[i].GetComponent<CanvasGroup>().alpha -= 0.1f;
        }

        canvas[i].GetComponent<CanvasGroup>().interactable = false;
        canvas[i].SetActive(false);

        StartCoroutine(FadeIn(j));

        yield return null;
    }

    IEnumerator FadeIn(int i)
    {
        canvas[i].SetActive(true);

        while (canvas[i].GetComponent<CanvasGroup>().alpha < 1)
        {
            yield return new WaitForSeconds(0.005f);
            canvas[i].GetComponent<CanvasGroup>().alpha += 0.1f;
        }

        canvas[i].GetComponent<CanvasGroup>().interactable = true;
        block.SetActive(false);

        yield return null;
    }

    private void CamMoveTo(int index)
    {
        const float allowedError = 0.001f;
        GameObject mainCam = Camera.main.gameObject;
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos[index].transform.position, Time.deltaTime * 4f);
        mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, camPos[index].transform.rotation, Time.deltaTime * 4f);

        if (Vector3.Distance(mainCam.transform.position, camPos[currentCamPos].transform.position) < allowedError &&
            Quaternion.Angle(mainCam.transform.rotation, camPos[currentCamPos].transform.rotation) < allowedError)
        {
            camMoving = false;
        }
    }

    public void ConfirmYes()
    {
        Application.Quit();
    }

    public void StartGamePlay()
    {
        GameManagerPC.Instance.LoadGamePlay();
    }

    public void Tutorial()
    {
        if (standbyTut.isReady)
        {
            GameManagerPC.Instance.LoadTutorial(standbyTut.player);
        }

    }

    public void PlayerReady(bool ready, int connectId)
    {

        if (GameManagerPC.Instance.state == GameManagerPC.GameState.Lobby)
        {
            bool allReady = true;

            for (int i = 0; i < standbyMulti.Length; i++)
            {

                if (standbyMulti[i].player != null)
                {

                    if (connectId == standbyMulti[i].player.playerId)
                    {
                        standbyMulti[i].OnPlayerReady(ready);
                    }

                    allReady &= standbyMulti[i].isReady;
                }
            }

            if (allReady)
            {
                StartGamePlay();
            }

        }
        else if (GameManagerPC.Instance.state == GameManagerPC.GameState.Tutorial)
        {
            if (standbyTut.player != null && connectId == standbyTut.player.playerId)
            {
                standbyTut.OnPlayerReady(ready);
                if (standbyTut.isReady) Tutorial();
            }
        }
    }

    private void ShowAllPlayerInMultiLobby()
    {
        for (int i = 0; i < GameManagerPC.Instance.playerManagement.playersInfo.Length; i++)
        {
            if (GameManagerPC.Instance.playerManagement.playersInfo[i] != null)
            {
                standbyMulti[i].ShowPlayer(GameManagerPC.Instance.playerManagement.playersInfo[i]);
            }
            else
            {
                standbyMulti[i].HidePlayer();
            }
        }
    }


}
