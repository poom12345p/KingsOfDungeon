using UnityEngine.Networking;
using UnityEngine;

public class StatNetworkMessage : MessageBase
{
    public int currentHp;
    public int maxHp;
    public float speed;
    public int attack;
}

public class PlayerNetworkMessage : MessageBase
{
    public string uid;
    public int id;
    public string playerName;
    public int level;
    public string job;
}

public class BooleanNetworkMessage : MessageBase
{
    public bool value;
}
