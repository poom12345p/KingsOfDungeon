using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{

    public PlayerRankManagement playerRank;
    private readonly string savePath = "/Kod.dat";

    private void Start()
    {
        var data = Load();
        playerRank = new PlayerRankManagement(data.playersSaved);
    }

    PlayerSavedInfo convertToPlayerSavedInfo(PlayerInfo info, string gameSession)
    {
        // store basic info
        PlayerSavedInfo savedInfo = new PlayerSavedInfo(
            info.playerUid,
            info.playerName,
            info.job,
            info.scoreTrack,
            gameSession
        );

        // Store item
        foreach (var item in info.ItemHold.Keys)
        {
            savedInfo.itemOwned.Add(new KeyValueSerializable<int, int>(
                item.itemModel.itemCode,
                info.ItemHold[item]
            ));
        }

        // store accuired skill
        foreach (var skillCode in info.acuiredSkill.Keys)
        {
            savedInfo.acuiredSkill.Add(new KeyValueSerializable<int, int>(
                skillCode,
                info.acuiredSkill[skillCode]
            ));
        }

        return savedInfo;
    }

    public void Save()
    {
        // Create or open a file to save
        FileStream file = new FileStream(Application.persistentDataPath + savePath, FileMode.OpenOrCreate);
        try
        {
            // print(Application.persistentDataPath);
            // BinaryFormatter to format data to binary
            BinaryFormatter bf = new BinaryFormatter();
            // Serialize mathode to write a file
            SavedData data = new SavedData();
            data.playersSaved = playerRank.savedData;
            bf.Serialize(file, data);

        }
        catch (SerializationException e)
        {
            Debug.LogError("Problem saving file: " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }

    public SavedData Load()
    {
        string filePath = Application.persistentDataPath + savePath;
        SavedData data = new SavedData();
        if (File.Exists(filePath))
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            try
            {

                BinaryFormatter bformatter = new BinaryFormatter();
                SavedData loadedData = (SavedData)bformatter.Deserialize(file);
                data = loadedData;

            }
            catch (SerializationException e)
            {
                Debug.LogError("Problem loading file: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }

        return data;
    }

    public void SavePlayerInfoInstance(PlayerInfo[] playerInfo, string gameSession)
    {
        var playerData = new List<PlayerSavedInfo>();

        var playerSaveList = new List<PlayerSavedInfo>();
        foreach (var player in playerInfo)
        {
            if (player != null)
            {
                playerSaveList.Add(convertToPlayerSavedInfo(player, gameSession));
            }
        }

        playerRank.AddPlayerData(playerSaveList);
        Save();
    }
}

[System.Serializable]
public class SavedData
{
    public List<PlayerSavedInfo> playersSaved;

    public SavedData()
    {
        playersSaved = new List<PlayerSavedInfo>();
    }
}

[System.Serializable]
public struct PlayerSavedInfo : System.IComparable<PlayerSavedInfo>
{
    public string playerUid;
    public string name;
    public string job;
    public string gameSession;
    public ScoreTrack score;

    public List<KeyValueSerializable<int, int>> itemOwned;
    public List<KeyValueSerializable<int, int>> acuiredSkill;

    public PlayerSavedInfo(string uid, string name, string job, ScoreTrack score, string gameSession)
    {
        playerUid = uid;
        this.name = name;
        this.job = job;
        this.score = score;
        this.gameSession = gameSession;

        itemOwned = new List<KeyValueSerializable<int, int>>();
        acuiredSkill = new List<KeyValueSerializable<int, int>>();
    }

    public int CompareTo(PlayerSavedInfo other)
    {
        return score.CalculateScoreValue() - other.score.CalculateScoreValue();
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return this.playerUid == ((PlayerSavedInfo)obj).playerUid;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return playerUid.GetHashCode();
    }
}

[System.Serializable]
public struct KeyValueSerializable<TKey, TValue>
{
    // use to store item and accuired skill 
    public TKey key;
    public TValue value;

    public KeyValueSerializable(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}
