using UnityEngine;

[System.Serializable]
public class PlayerManagement
{

    public static readonly int maxPlayer = 4;
    public PlayerInfo[] playersInfo;
    public GameObject[] playerInstance;

    public int connectedPlayer = 0;

    public PlayerManagement()
    {
        playersInfo = new PlayerInfo[maxPlayer];
        playerInstance = new GameObject[maxPlayer];

    }

    public int PlayerConnected(PlayerInfo playerConnected)
    {

        // player reconnect while playing
        if (playerConnected.playerId >= 0 && GameManagerPC.Instance.state == GameManagerPC.GameState.GamePlay)
        {

            playersInfo[playerConnected.playerId].isDisconnected = false;
            playersInfo[playerConnected.playerId].connectionChannel = playerConnected.connectionChannel;
            // print("player reconnected "+ playerConnected);
            // connectedPlayer++;
            Debug.Log("Player Reconnected: " + playersInfo[playerConnected.playerId]);

            MyServer.Instance.SendBufferedMessageToClient(playerConnected.playerId);

            foreach (var instance in playerInstance)
            {
                unitHitbox unit = instance.GetComponent<unitHitbox>();
                if (unit.id == playerConnected.playerId)
                {
                    unit.statContol.PlayerStatChange();
                    break;
                }
            }


            return playersInfo[playerConnected.playerId].playerId;
        }

        int playerIndex = FindPlayerByID(playerConnected.playerId);

        if (playerIndex < 0)
        {
            // create new player
            for (int i = 0; i < playersInfo.Length; i++)
            {
                if (playersInfo[i] == null)
                {
                    playersInfo[i] = playerConnected;
                    playerIndex = i;
                    connectedPlayer++;
                    break;
                }
            }
        }
        else
        {
            // update player
            // also use for ready in lobby
            playersInfo[playerIndex] = playerConnected;
        }

        return playerIndex;
    }

    public int ManagePlayerDisconnected(int connectId)
    {
        // when disconnected clear player data
        for (int i = 0; i < playersInfo.Length; i++)
        {
            if (playersInfo[i] != null && playersInfo[i].connectionChannel == connectId)
            {
                if (GameManagerPC.Instance.state == GameManagerPC.GameState.Menu ||
                GameManagerPC.Instance.state == GameManagerPC.GameState.Lobby ||
                GameManagerPC.Instance.state == GameManagerPC.GameState.Tutorial)
                {
                    playersInfo[i] = null;
                    connectedPlayer--;
                }
                else
                {
                    // disconnect while playing
                    //print("player disconected " + playersInfo[i]);
                    playersInfo[i].isDisconnected = true;
                    playersInfo[i].connectionChannel = -1;
                }

                //print("player disconnected "+ playersInfo[i]);
                Debug.Log("isDisconnected " + playersInfo[i].isDisconnected);

                return i;
            }
        }

        return -1;
    }

    public int FindPlayerByID(int playerId)
    {
        for (int i = 0; i < playersInfo.Length; i++)
        {
            if (playersInfo[i] != null && playersInfo[i].playerId == playerId)
            {
                return i;
            }
        }

        // not found
        return -1;
    }

    public int FindPlayerByConnectionChannel(int connectionChannel)
    {
        for (int i = 0; i < playersInfo.Length; i++)
        {
            if (playersInfo[i] != null && playersInfo[i].connectionChannel == connectionChannel)
            {
                return i;
            }
        }

        // not found
        return -1;
    }

    public PlayerInfo GetPlayerByID(int playerId)
    {
        int playerIndex = FindPlayerByID(playerId);
        if (playerIndex < 0) return null;
        return playersInfo[playerIndex];
    }

    public PlayerInfo GetPlayerByConnectionChannel(int connectionChannel)
    {
        int playerIndex = FindPlayerByConnectionChannel(connectionChannel);
        if (playerIndex < 0) return null;
        return playersInfo[playerIndex];
    }

    // use in tutorial lobby to get the fisrt player that connect
    public PlayerInfo GetFirstConnectedPlayer()
    {

        for (int i = 0; i < playersInfo.Length; i++)
        {
            if (playersInfo[i] != null)
            {
                return playersInfo[i];
            }
        }

        return null;
    }

    public bool IsMaximumPlayer()
    {
        return connectedPlayer == maxPlayer;
    }

    public int CountDeadPlayer()
    {
        int deadPlayer = 0;

        for (int i = 0; i < 4; i++)
        {
            if (playersInfo[i] != null) deadPlayer = playersInfo[i].isDead ? deadPlayer + 1 : deadPlayer;
        }

        return deadPlayer;
    }

    public void AddKillScore(int playerId)
    {
        int playerIndex = FindPlayerByID(playerId);
        if (playerIndex < 0) return;
        playersInfo[playerIndex].scoreTrack.monsterKilled += 1;
    }

    public void AddItemScore(int playerId)
    {
        int playerIndex = FindPlayerByID(playerId);
        if (playerIndex < 0) return;
        playersInfo[playerIndex].scoreTrack.itemOwned += 1;
    }

    public void RemoveItemScore(int playerId)
    {
        int playerIndex = FindPlayerByID(playerId);
        if (playerIndex < 0) return;
        playersInfo[playerIndex].scoreTrack.itemOwned -= 1;
    }

    public void AddMapScore(int playerId, int map_score)
    {
        int playerIndex = FindPlayerByID(playerId);
        if (playerIndex < 0) return;
        playersInfo[playerIndex].scoreTrack.mapPassed = map_score;
    }


}
