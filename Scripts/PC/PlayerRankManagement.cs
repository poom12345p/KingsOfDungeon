using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRankManagement
{

    public List<PlayerSavedInfo> savedData;
    private Dictionary<int, List<PlayerSavedInfo>> savedDataDictionary = new Dictionary<int, List<PlayerSavedInfo>>();

    const int limitTeamEachState = 5;

    public PlayerRankManagement(List<PlayerSavedInfo> savedData)
    {
        this.savedData = savedData;
        savedDataDictionary = ConvertToDictionary(savedData);
    }

    public Dictionary<int, List<PlayerSavedInfo>> ConvertToDictionary(List<PlayerSavedInfo> savedData)
    {
        var converted = new Dictionary<int, List<PlayerSavedInfo>>();

        foreach (var playerSavedInfo in savedData)
        {
            int statePassed = playerSavedInfo.score.mapPassed;
            if (converted.ContainsKey(statePassed)) converted[statePassed].Add(playerSavedInfo);
            else
            {
                var saveList = new List<PlayerSavedInfo>();
                saveList.Add(playerSavedInfo);
                converted.Add(statePassed, saveList);
            }
        }

        return converted;
    }

    public void AddPlayerData(PlayerSavedInfo newPlayerData)
    {
        int state = newPlayerData.score.mapPassed;
        if (savedDataDictionary.ContainsKey(state))
        {
            var saveList = savedDataDictionary[state];
            saveList.Add(newPlayerData);
            savedData.Add(newPlayerData);
        }
        else
        {
            var saveList = new List<PlayerSavedInfo>();
            saveList.Add(newPlayerData);
            savedDataDictionary.Add(state, saveList);
            savedData.Add(newPlayerData);
        }
    }

    public void AddPlayerData(List<PlayerSavedInfo> newPlayerData)
    {
        int state = newPlayerData[0].score.mapPassed;
        if (savedDataDictionary.ContainsKey(state))
        {
            var oldTeam = FindPlayerTeamExistInState(state, newPlayerData);

            int sumScore = GetScoreFromSavedList(newPlayerData);

            if (oldTeam != null)
            {
                int oldTeamSumScore = GetScoreFromSavedList(oldTeam);

                if (sumScore <= oldTeamSumScore) return;
                else
                {
                    // remove old team from data
                    RemovePlayerSaveList(oldTeam);
                }
            }
            else if (CountTeamInState(state) >= limitTeamEachState)
            {
                // TODO: check high score
                var playerTeamInState = GetPlayerTeamInState(state);
                List<KeyValuePair<string, int>> teamScore = new List<KeyValuePair<string, int>>();
                foreach (var team in playerTeamInState)
                {
                    int sum = GetScoreFromSavedList(team.Value);
                    teamScore.Add(
                        new KeyValuePair<string, int>(team.Key, sum)
                    );
                }
                // if(playerTeamInState.Count)
                int sumNew = GetScoreFromSavedList(newPlayerData);
                teamScore.Add(new KeyValuePair<string, int>(newPlayerData[0].gameSession, sumNew));

                teamScore.Sort((t1, t2) => t2.Value - t1.Value);

                var teamOut = teamScore[limitTeamEachState];

                if (playerTeamInState.ContainsKey(teamOut.Key))
                {
                    RemovePlayerSaveList(playerTeamInState[teamOut.Key]);
                }
                else
                {
                    return;
                }
            }
        }


        foreach (PlayerSavedInfo save in newPlayerData)
        {
            AddPlayerData(save);
        }

    }

    public bool PlayerExistsInState(int state)
    {
        foreach (var entry in savedDataDictionary)
        {
            Debug.Log("In State: " + entry.Key + ",Have " + entry.Value.Count + " Player");
        }
        return savedDataDictionary.ContainsKey(state);
    }

    public List<PlayerSavedInfo> GetPlayersInState(int state)
    {
        var saveList = new List<PlayerSavedInfo>();
        if (!savedDataDictionary.TryGetValue(state, out saveList)) Debug.LogWarning("No Player in that state yet");
        return saveList;
    }

    public Dictionary<string, List<PlayerSavedInfo>> GetPlayersTeam(List<PlayerSavedInfo> data)
    {
        var result = new Dictionary<string, List<PlayerSavedInfo>>();

        if (data != null)
        {
            // Get player that play the same game
            foreach (var playerSave in data)
            {
                if (result.ContainsKey(playerSave.gameSession)) result[playerSave.gameSession].Add(playerSave);
                else
                {
                    var playersSaveList = new List<PlayerSavedInfo>();
                    playersSaveList.Add(playerSave);
                    result.Add(playerSave.gameSession, playersSaveList);
                }
            }
        }

        return result;
    }

    public Dictionary<string, List<PlayerSavedInfo>> GetPlayerTeamInState(int state)
    {
        var playerInState = GetPlayersInState(state);
        return GetPlayersTeam(playerInState);
    }

    public List<PlayerSavedInfo> GetHighestRankTeamInState(int state)
    {
        var savedList = new List<PlayerSavedInfo>();
        try
        {

            var playerTeamsInState = GetPlayerTeamInState(state);

            int highestScore = 0;

            foreach (var playersInTeam in playerTeamsInState.Values)
            {
                int currentTeamScore = GetScoreFromSavedList(playersInTeam);
                if (currentTeamScore > highestScore)
                {
                    highestScore = currentTeamScore;
                    savedList = playersInTeam;
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Error Happend When Trying to GetHighestRankTeamInState: " + e.Message);
        }

        return savedList;
    }

    public List<PlayerSavedInfo> FindPlayerTeamExistInState(int state, List<PlayerSavedInfo> newPlayersTeam)
    {
        var playersInState = GetPlayerTeamInState(state);
        try
        {
            foreach (List<PlayerSavedInfo> oldPlayerTeam in playersInState.Values)
            {
                if (newPlayersTeam.Count == oldPlayerTeam.Count)
                {
                    List<PlayerSavedInfo> oldTeam = new List<PlayerSavedInfo>();
                    for (int i = 0; i < newPlayersTeam.Count; i++)
                    {
                        bool hasPerson = false;
                        for (int j = 0; j < oldPlayerTeam.Count; j++)
                        {
                            if (newPlayersTeam[i].playerUid == oldPlayerTeam[j].playerUid)
                            {
                                oldTeam.Add(oldPlayerTeam[j]);
                                hasPerson = true;
                                break;
                            }
                        }

                        if (!hasPerson) break;
                    }

                    if (oldTeam.Count == newPlayersTeam.Count) return oldTeam;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error happen when Finding Team Exists: " + e.Message);
        }

        return null;

    }

    public int CountTeamInState(int state)
    {
        return GetPlayerTeamInState(state).Count;
    }

    public void RemovePlayerSaveList(List<PlayerSavedInfo> playersSave)
    {
        int count = playersSave.Count;
        try
        {
            for (int i = 0; i < count; i++)
            {
                if (!savedData.Remove(playersSave[i]))
                {
                    Debug.LogError("Can't remove save not in data");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error happen when RemovePlayerSaveList " + e.Message);
        }

        savedDataDictionary = ConvertToDictionary(savedData);
    }

    public int GetScoreFromSavedList(List<PlayerSavedInfo> savedInfo)
    {
        int score = 0;

        savedInfo.ForEach(p => score += p.score.CalculateScoreValue());

        return score;
    }

}
