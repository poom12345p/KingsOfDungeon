using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{

    // for communication between server and client
    public int connectionChannel;


    public int playerId;

    // uniqe id of a player
    public string playerUid;
    public string playerName;
    public int level;

    // current job
    public string job;

    // dead
    public bool isDead;

    public bool isDisconnected;
    public ScoreTrack scoreTrack;

    public Dictionary<int, int> acuiredSkill = new Dictionary<int, int>();
    public Dictionary<ItemBase, int> ItemHold = new Dictionary<ItemBase, int>();

    public PlayerInfo(string playerUid, string name, int level, string job)
    {
        this.playerUid = playerUid;
        playerId = -1;
        connectionChannel = -1;
        this.playerName = name;
        this.level = level;
        this.job = job;
        isDead = false;
        resetScore();
    }

    public PlayerInfo(string playerUid, int id, int connectedId, string name, int level, string job)
    {
        this.playerUid = playerUid;
        playerId = id;
        connectionChannel = connectedId;
        this.playerName = name;
        this.level = level;
        this.job = job;
        isDead = false;
        resetScore();

    }

    public void PlayerUpSkill(int skillCode)
    {
        if (acuiredSkill.ContainsKey(skillCode))
            acuiredSkill[skillCode] += 1;
        else
            acuiredSkill.Add(skillCode, 1);
    }

    public override string ToString()
    {
        return string.Format("name:{0}, id:{1}, connectionId:{2}, job:{3}, isDisconnected:{4}, isDead:{5}, score:{6}", playerName, playerId, connectionChannel, job, isDisconnected, isDead, scoreTrack);
    }

    public void resetScore()
    {
        scoreTrack = new ScoreTrack(0);
    }
}

[System.Serializable]
public struct ScoreTrack
{
    public int monsterKilled;
    public int itemOwned;
    public int mapPassed;

    public ScoreTrack(int initvalue)
    {
        monsterKilled = initvalue;
        itemOwned = initvalue;
        mapPassed = initvalue;
    }

    // TODO: calculate value better
    public int CalculateScoreValue()
    {
        return monsterKilled * 10 + itemOwned * 2 + mapPassed * 1000;
    }

}
