using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MyClient : MonoBehaviour
{

    // singleton
    public static MyClient Instance { get; private set; }
    static NetworkClient client;

    MyNetworkDiscovery networkDiscovery;

    // ipaddress
    public string ip;
    public int port = 25000;

    public bool canVibrate;

    // void OnGUI()
    // {
    //     // GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipaddress);
    //     if (GameManagerMobile.Instance.playerInfo != null) GUI.Label(new Rect(20, Screen.height - 35, 100, 20), GameManagerMobile.Instance.playerInfo.playerId.ToString());
    //     GUI.Label(new Rect(30, Screen.height - 15, 100, 20), client.isConnected.ToString());
    //     // if (!client.isConnected) if (GUI.Button(new Rect(10, 10, 60, 50), "Connect")) ;
    // }
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    void Start()
    {

        client = new NetworkClient();

        if (networkDiscovery == null) networkDiscovery = GetComponent<MyNetworkDiscovery>();
        //if (networkDiscovery == null) throw new System.Exception();

        RegisterHandlers();
    }



    // handle all register things
    void RegisterHandlers()
    {
        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        client.RegisterHandler(NetworkMessageNumber.msgChangeState, ReceiveServerChangeState);
        client.RegisterHandler(NetworkMessageNumber.msgPlayerConnected, ReceiveServerPlayerConnected);
        client.RegisterHandler(NetworkMessageNumber.msgPlayerStatus, RecievePlayerStatus);
        client.RegisterHandler(NetworkMessageNumber.msgItemData, RecieveItem);
        client.RegisterHandler(NetworkMessageNumber.msgRemoveItemData, RecieveRemoveItem);
        client.RegisterHandler(NetworkMessageNumber.msgStatChange, RecieveStatsChange);
        client.RegisterHandler(NetworkMessageNumber.msgLoadingState, RecieveLoadingStateOfServer);
    }

    void OnConnected(NetworkMessage message)
    {
        GameManagerMobile.Instance.HideDisconnectedPage();
    }

    // RECEIVE MESSAGE
    void OnDisconnected(NetworkMessage message)
    {
        // handle thing when disconnected to server
        // Debug.Log("diconnected From Server " + message.conn.connectionId);
        GameManagerMobile.Instance.DisconnectedFromServer();

    }

    void ReceiveServerChangeState(NetworkMessage message)
    {
        StringMessage messageContainer = new StringMessage
        {
            value = message.ReadMessage<StringMessage>().value
        };

        string[] deltas = messageContainer.value.Split('|');


        if (deltas[0].Equals("MultiPlay"))
        {
            GameManagerMobile.Instance.gameSession = deltas[1];
            GameManagerMobile.Instance.StartQuickPlay();
        }
        else if (deltas[0].Equals("Tutorial"))
        {
            // Tutorial
            GameManagerMobile.Instance.gameSession = deltas[1];
            GameManagerMobile.Instance.StartTutorial();

        }
        else if (deltas[0].Equals("GameEnd"))
        {
            // back to menu
            GameManagerMobile.Instance.BackToMeu();
        }

        //send ackknowledge back to server ?
    }

    void ReceiveServerPlayerConnected(NetworkMessage message)
    {

        StringMessage msg = message.ReadMessage<StringMessage>();
        string[] deltas = msg.value.Split('|');

        if (deltas[0].Equals("NotAllowed"))
        {
            client.Disconnect();
            GameManagerMobile.Instance.ResetVariable();
            GameManagerMobile.Instance.playerNumber = -1;
            // Debug.LogError("Can't connect server not allowed");
        }
        else if (deltas[0].Equals("Allowed"))
        {
            GameManagerMobile.Instance.playerInfo.playerId = int.Parse(deltas[1]);
            GameManagerMobile.Instance.playerInfo.connectionChannel = message.conn.connectionId;
            GameManagerMobile.Instance.playerNumber = int.Parse(deltas[2]);

            SendPlayerInfo();

            if (GameManagerMobile.Instance.state == GameManagerMobile.GameState.Controller ||
                GameManagerMobile.Instance.state == GameManagerMobile.GameState.TutorialController)
            {
                GameManagerMobile.Instance.BackToMeu();
            }
            else
            {
                GameManagerMobile.Instance.ConnectedSuccessfull();
            }

        }
        else if (deltas[0].Equals("G"))
        {
            if (GameManagerMobile.Instance.playerInfo.playerId < 0 || !GameManagerMobile.Instance.gameSession.Equals(deltas[1]))
            {
                client.Disconnect();
                GameManagerMobile.Instance.ResetVariable();
                GameManagerMobile.Instance.playerNumber = -1;
                //Debug.LogError("Game Still Playing, You can't interrupt them");
            }
            else
            {
                //Debug.Log("You reconnect to server");
                SendPlayerInfo();
                GameManagerMobile.Instance.ConnectedSuccessfull();
            }
        }

    }

    void RecievePlayerStatus(NetworkMessage message)
    {
        StringMessage msg = message.ReadMessage<StringMessage>();

        if (msg.value.Equals("DEAD"))
        {
            GameManagerMobile.Instance.PlayerIsDead(true);
        }
        else if (msg.value.Equals("REVIVE"))
        {
            GameManagerMobile.Instance.PlayerIsDead(false);
        }

        else
        {
            switch (msg.value)
            {
                case "ATTACKED":
                    //if (canVibrate) Handheld.Vibrate();
                    break;
                default:
                    GameManagerMobile.Instance.myController.receiveMsg(msg.value);
                    break;
            }

        }
    }

    void RecieveItem(NetworkMessage message)
    {
        IntegerMessage msg = message.ReadMessage<IntegerMessage>();
        int itemCode = msg.value;

        GameManagerMobile.Instance.StoreItemInInventory(itemCode);
    }

    void RecieveRemoveItem(NetworkMessage message)
    {

        IntegerMessage msg = message.ReadMessage<IntegerMessage>();
        int itemCode = msg.value;

        GameManagerMobile.Instance.RemoveItemInInventory(itemCode);
    }

    void RecieveStatsChange(NetworkMessage message)
    {
        StatNetworkMessage msg = message.ReadMessage<StatNetworkMessage>();
        PlayerStatMobile stat = new PlayerStatMobile(
            msg.maxHp,
            msg.currentHp,
            msg.attack,
            msg.speed
        );
        GameManagerMobile.Instance.PlayerStatChange(stat);
    }

    void RecieveLoadingStateOfServer(NetworkMessage message)
    {
        BooleanNetworkMessage msg = message.ReadMessage<BooleanNetworkMessage>();
        bool serverIsLoading = msg.value;
        GameManagerMobile.Instance.OnLoadingStateOfServerChange(serverIsLoading);
    }


    // SENDMESSAGE
    // SENDING MESSAGE TO SERVER
    public static void SendButtonInfo(string action)
    {
        // Debug.Log("send: " + action);
        if (GameManagerMobile.Instance.playerInfo.playerId >= 0)
        {
            StringMessage msg = new StringMessage();
            msg.value = action + "|" + GameManagerMobile.Instance.playerInfo.playerId;
            client.Send(NetworkMessageNumber.msgBtnClickedId, msg);
        }
    }

    // SENDING READY TO SERVER
    public static void SendMessageReady()
    {
        StringMessage msg = new StringMessage();
        msg.value = GameManagerMobile.Instance.ready.ToString() + "|" + GameManagerMobile.Instance.playerInfo.playerId;
        client.Send(NetworkMessageNumber.msgSendPlayerReady, msg);
    }

    // SENDING JOYSTICK INFO
    public static void SendJoystickInfo(float hDelta, float vDelta)
    {

        if (client.isConnected)
        {
            StringMessage msg = new StringMessage
            {
                value = hDelta + "|" + vDelta + "|" + GameManagerMobile.Instance.playerInfo.playerId
            };

            // faster
            client.Send(NetworkMessageNumber.msgJoyStickInfo, msg);
            //client.Send(msgJoyStickInfo, msg);
        }
    }

    // SENDING PLAYER INFO
    public static void SendPlayerInfo()
    {
        PlayerNetworkMessage msg = new PlayerNetworkMessage
        {
            uid = GameManagerMobile.Instance.playerInfo.playerUid,
            id = GameManagerMobile.Instance.playerInfo.playerId,
            playerName = GameManagerMobile.Instance.playerInfo.playerName,
            level = GameManagerMobile.Instance.playerInfo.level,
            job = GameManagerMobile.Instance.playerInfo.job
        };
        // Debug.Log("SendPlayerInfo");
        client.Send(NetworkMessageNumber.msgSendPlayerInfo, msg);

    }

    public static void SendUseItemMessage(int itemCode)
    {
        IntegerMessage msg = new IntegerMessage
        {
            value = itemCode
        };
        client.Send(NetworkMessageNumber.msgUseItem, msg);
    }

    public static void SendUpSkillMessage(int skillCode)
    {
        IntegerMessage msg = new IntegerMessage
        {
            value = skillCode
        };
        client.Send(NetworkMessageNumber.msgUpSkill, msg);
    }


    //use to reconnect
    public void Connect()
    {
        client.Connect(ip.TrimEnd(), port);
    }

    //use to initiate connect
    public void Connect(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        client.Connect(ip.TrimEnd(), port);
    }
    public void ClickConnect(InputField iptext)
    {
        this.ip = iptext.text;
        client.Connect(ip.TrimEnd(), port);
    }


    public bool IsConnectedToServer()
    {
        return client.isConnected;
    }

    public void DisconnectServer()
    {
        client.Disconnect();
    }

    public void ListenForRoom()
    {
        networkDiscovery.StartListenOnBoardCast();
    }

    public void StopListen()
    {
        networkDiscovery.EndListening();
    }

    public void toggleVibrate()
    {
        canVibrate = !canVibrate;
    }
}


