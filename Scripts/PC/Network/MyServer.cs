using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityStandardAssets.CrossPlatformInput;


public class MyServer : MonoBehaviour
{

    public static MyServer Instance { get; private set; }
    public string ipAddress;
    CrossPlatformInputManager.VirtualAxis[] HRZTAxis = new CrossPlatformInputManager.VirtualAxis[4],
                                            VTCAxis = new CrossPlatformInputManager.VirtualAxis[4];

    string hrztaxis_n = "Horizontal_", vtcaxis_n = "Vertical_";

    public int port = 25000;

    public PlayersCommander playersComand;

    [Tooltip("Check this to true if you're testing Things")]
    public bool test = true;

    MyNetworkDiscovery networkDiscovery;

    private List<KeyValuePair<int, BufferedMessage>> messageBuffer;

    struct BufferedMessage
    {
        public MessageBase message;
        public short messageType;

        public BufferedMessage(MessageBase message, short messageType)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }

    // void OnGUI()
    // {
    //     string ipaddress = LocalIPAddress();
    //     GUI.Box(new Rect(10, Screen.height - 50, 100, 50), LocalIPAddress());
    //     GUI.Label(new Rect(20, Screen.height - 35, 100, 20), "Status:" + NetworkServer.active);
    //     GUI.Label(new Rect(20, Screen.height - 20, 100, 20), "Connected:" + NetworkServer.connections.Count);
    // }

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        messageBuffer = new List<KeyValuePair<int, BufferedMessage>>();
        if (networkDiscovery == null) networkDiscovery = gameObject.GetComponent<MyNetworkDiscovery>();
        // if (networkDiscovery == null) throw new System.Exception();

        // running server in background
        Application.runInBackground = true;
        ipAddress = LocalIPAddress();

        for (int i = 0; i < 4; i++)
        {
            HRZTAxis[i] = new CrossPlatformInputManager.VirtualAxis(hrztaxis_n + i); CrossPlatformInputManager.RegisterVirtualAxis(HRZTAxis[i]);
            VTCAxis[i] = new CrossPlatformInputManager.VirtualAxis(vtcaxis_n + i); CrossPlatformInputManager.RegisterVirtualAxis(VTCAxis[i]);
        }

        bool networkStart = false;
        do
        {
            try
            {
                networkStart = NetworkServer.Listen(port);
            }
            catch (System.Exception err)
            {
                Debug.LogWarning("Start Server Not complete: " + err + "Try Changing port");
                port++;
            }
        }
        while (!networkStart);

        //Debug.Log("listening at port: " + port);
        RegisterHandlers();

    }

    // handle all register things
    private void RegisterHandlers()
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);

        // Our message type.
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgBtnClickedId, ServerReceiveMessageButton);
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgSendPlayerInfo, OnMessageReceivedPlayerInfo);
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgSendPlayerReady, OnMessageReceivedPlayerReady);
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgJoyStickInfo, OnMessageReceivedJoyStickInfo);
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgUseItem, OnMessageReceivedUseItem);
        NetworkServer.RegisterHandler(NetworkMessageNumber.msgUpSkill, OnMessageReceivedUpSkill);
    }

    // RECIEVED MASSAGE FROM CLIENT
    void OnClientConnected(NetworkMessage massage)
    {
        // Do stuff when a client connects to this server
        //Debug.Log("Client Connected " + massage.conn.connectionId);

        if (GameManagerPC.Instance.state == GameManagerPC.GameState.Lobby ||
            (
            GameManagerPC.Instance.state == GameManagerPC.GameState.Tutorial &&
            GameManagerPC.Instance.playerManagement.connectedPlayer < 1
            ) &&
            !GameManagerPC.Instance.playerManagement.IsMaximumPlayer())
        {

            StringMessage message = new StringMessage()
            {
                value = "Allowed|" + (massage.conn.connectionId - 1) + "|" + GameManagerPC.Instance.playerManagement.connectedPlayer
            };
            NetworkServer.SendToClient(massage.conn.connectionId, NetworkMessageNumber.msgPlayerConnected, message);
        }
        else if (GameManagerPC.Instance.state == GameManagerPC.GameState.GamePlay ||
                GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
        {
            // Check to Reconnect to server
            StringMessage message = new StringMessage()
            {
                value = "G|" + GameManagerPC.Instance.gameSession
            };
            NetworkServer.SendToClient(massage.conn.connectionId, NetworkMessageNumber.msgPlayerConnected, message);
        }
        else
        {
            StringMessage message = new StringMessage()
            {
                value = "NotAllowed"
            };
            NetworkServer.SendToClient(massage.conn.connectionId, NetworkMessageNumber.msgPlayerConnected, message);

        }

    }

    public void OnMessageReceivedJoyStickInfo(NetworkMessage massage)
    {

        StringMessage msg = new StringMessage();
        msg.value = massage.ReadMessage<StringMessage>().value;

        string[] deltas = msg.value.Split('|');
        HRZTAxis[int.Parse(deltas[2])].Update(float.Parse(deltas[0]));
        VTCAxis[int.Parse(deltas[2])].Update(float.Parse(deltas[1]));
    }

    void ServerReceiveMessageButton(NetworkMessage massage)
    {
        StringMessage msg = new StringMessage();
        msg.value = massage.ReadMessage<StringMessage>().value;
        string[] deltas = msg.value.Split('|');
        if (playersComand != null)
            playersComand.sendComand(int.Parse(deltas[1]), deltas[0]);
    }

    void OnClientDisconnected(NetworkMessage massage)
    {
        // Debug.Log("Client disconnected " + massage.conn.connectionId);
        GameManagerPC.Instance.OnPlayerDisconnected(massage.conn.connectionId);
    }

    void OnMessageReceivedPlayerInfo(NetworkMessage massage)
    {
        //        Debug.Log(" OnMessageReceivedPlayerInfo");
        PlayerNetworkMessage readMessage = massage.ReadMessage<PlayerNetworkMessage>();

        // if id >= 0 player has been connected before; 
        int playerId = readMessage.id >= 0 ? readMessage.id : (massage.conn.connectionId - 1);

        PlayerInfo player = new PlayerInfo(
           readMessage.uid,
           playerId,
           massage.conn.connectionId,
           readMessage.playerName,
           readMessage.level,
           readMessage.job
        );

        GameManagerPC.Instance.OnPlayerConnectedToGame(player);
        //for test
        if (playersComand != null && test)
        {
            playersComand.createCharector();
            // Debug.Log("Create player test");
        }
    }

    void OnMessageReceivedPlayerReady(NetworkMessage massage)
    {
        StringMessage ready = massage.ReadMessage<StringMessage>();
        string[] deltas = ready.value.Split('|');

        if (deltas[0].Equals("True"))
        {
            GameManagerPC.Instance.PlayerPressReady(true, int.Parse(deltas[1]));
        }
        else if (deltas[0].Equals("False"))
        {
            GameManagerPC.Instance.PlayerPressReady(false, int.Parse(deltas[1]));
        }
    }

    void OnMessageReceivedUseItem(NetworkMessage networkMessage)
    {
        IntegerMessage message = networkMessage.ReadMessage<IntegerMessage>();
        int itemCode = message.value;
        // if (GameManagerPC.Instance != null)
        //     GameManagerPC.Instance.RequestUsingItem(itemCode, networkMessage.conn.connectionId);
    }

    void OnMessageReceivedUpSkill(NetworkMessage networkMessage)
    {
        IntegerMessage message = networkMessage.ReadMessage<IntegerMessage>();
        int skillCode = message.value;
        int id = -1;
        foreach (var player in GameManagerPC.Instance.playerManagement.playersInfo)
        {
            if (player != null && player.connectionChannel == networkMessage.conn.connectionId)
            {
                id = player.playerId;
                player.PlayerUpSkill(skillCode);
                break;
            }
        }
        playersComand.sendUpSkill(id, skillCode);
    }

    // SEND MESSAGE TO CLIENT
    public void SendChangeStateMessage(string state)
    {
        StringMessage massage = new StringMessage();
        massage.value = state + "|" + GameManagerPC.Instance.gameSession;
        //        Debug.Log("send: " + massage.value);
        NetworkServer.SendToAll(NetworkMessageNumber.msgChangeState, massage);
    }

    public void SendPlayerStatus(string status, PlayerInfo playerInfo)
    {
        StringMessage massage = new StringMessage();
        massage.value = status;

        //if (CanSentToClient(message, connectionId))
        //NetworkServer.SendToClient(connectionId, NetworkMessageNumber.msgPlayerStatus, massage);
        SentMessageToClient(massage, NetworkMessageNumber.msgPlayerStatus, playerInfo);
    }

    public void SendItemDataToClient(int itemCode, PlayerInfo playerInfo)
    {
        //print("send "+itemCode+" to client");
        IntegerMessage message = new IntegerMessage();
        message.value = itemCode;

        //if (CanSentToClient(message, connectionId))
        //NetworkServer.SendToClient(connectionId, NetworkMessageNumber.msgItemData, message);
        SentMessageToClient(message, NetworkMessageNumber.msgItemData, playerInfo);
    }

    public void SendRemoveItemDataToClient(int itemCode, PlayerInfo playerInfo)
    {
        IntegerMessage message = new IntegerMessage();
        message.value = itemCode;


        //NetworkServer.SendToClient(connectionId, NetworkMessageNumber.msgRemoveItemData, message);
        SentMessageToClient(message, NetworkMessageNumber.msgRemoveItemData, playerInfo);
    }

    public void SendPlayerStatChangeMessage(PlayerInfo playerInfo, int maxHp, int currentHp, int attack, float speed)
    {
        StatNetworkMessage message = new StatNetworkMessage();
        message.maxHp = maxHp;
        message.currentHp = currentHp;
        message.attack = attack;
        message.speed = speed;

        SentMessageToClient(message, NetworkMessageNumber.msgStatChange, playerInfo);
        //NetworkServer.SendToClient(connectionId, NetworkMessageNumber.msgStatChange, message);
    }

    public void SendMessageLoadingState(bool isLoading)
    {
        BooleanNetworkMessage message = new BooleanNetworkMessage();
        message.value = isLoading;

        NetworkServer.SendToAll(NetworkMessageNumber.msgLoadingState, message);
    }

    // Buffer handler
    private void SentMessageToClient(MessageBase message, short messageType, PlayerInfo playerInfo)
    {
        if (!playerInfo.isDisconnected)
        {
            NetworkServer.SendToClient(playerInfo.connectionChannel, messageType, message);
        }
        else
        {
            // send when client connected back
            messageBuffer.Add(new KeyValuePair<int, BufferedMessage>(playerInfo.playerId, new BufferedMessage(message, messageType)));
        }
    }

    public void SendBufferedMessageToClient(int playerId)
    {
        int connectionChannel = GameManagerPC.Instance.playerManagement.GetPlayerByID(playerId).connectionChannel;
        int count = messageBuffer.Count;
        List<BufferedMessage> removeMessage = new List<BufferedMessage>();
        foreach (var message in messageBuffer)
        {
            if (message.Key == playerId)
            {
                removeMessage.Add(message.Value);
                NetworkServer.SendToClient(connectionChannel, message.Value.messageType, message.Value.message);
            }
        }

        foreach (var remove in removeMessage)
        {
            var messageToRemove = messageBuffer.Find(p => p.Value.message == remove.message && p.Value.messageType == remove.messageType);

            messageBuffer.Remove(messageToRemove);
        }
    }

    public void ClearBufferedMessage()
    {
        messageBuffer.Clear();
    }

    // Get Ip Adrress
    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {

                localIP = ip.ToString();
                break;
            }
        }
        return localIP;

    }

    public void StartLobbyBroadcasting(string roomName)
    {
        //        print(networkDiscovery);
        networkDiscovery.StartBoardCast(roomName, port);
    }

    public void EndLobbyBroardcasting()
    {
        networkDiscovery.EndBroadCasting();
    }

    public bool IsBroadCast()
    {
        if (networkDiscovery != null)
        {
            return networkDiscovery.running;
        }
        return false;
    }

    public void GetPlayerComander(PlayersCommander pcmd)
    {
        playersComand = pcmd;


    }

    public void DisconnectAllClient()
    {

        foreach (var connection in NetworkServer.connections)
        {
            try
            {
                connection.Disconnect();
            }
            catch (System.Exception e)
            {
                print(e);
            }

        }

    }

    //shutdoen server when quit
    void OnApplicationQuit()
    {
        NetworkServer.Shutdown();
    }

    // handle Error when sending message
    private void OnError(NetworkMessage netMsg)
    {
        ErrorMessage errorMsg = new ErrorMessage();
        errorMsg.Deserialize(netMsg.reader);

        Debug.LogError("NetworkServerError errorCode: " + errorMsg.errorCode);
    }


}


