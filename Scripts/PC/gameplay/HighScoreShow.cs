using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreShow : MonoBehaviour
{

    public GameObject content;
    public HighScoreTile highScoreTilePrefab;

    public List<HighScoreTile> scoreTiles;

    [SerializeField]
    private int highScoreCount = 10;


    public void ShowAllHighScore()
    {
        var playerRank = GameManagerPC.Instance.saveManager.playerRank;
        var savedInfoTeam = playerRank.GetPlayersTeam(playerRank.savedData);
        var listTeam = new List<KeyValuePair<string, List<PlayerSavedInfo>>>();

        foreach (var item in savedInfoTeam)
        {
            listTeam.Add(item);
        }

        listTeam.Sort((l1, l2) => playerRank.GetScoreFromSavedList(l2.Value) - playerRank.GetScoreFromSavedList(l1.Value));

        ShowHighScoreFromSave(listTeam);
    }

    public void ShowHighScoreFromSave(List<KeyValuePair<string, List<PlayerSavedInfo>>> allPlayersTeamInfo)
    {
        for (int i = 0; i < allPlayersTeamInfo.Count && i < highScoreCount; i++)
        {
            var info = allPlayersTeamInfo[i];
            HighScoreTile scoreTile = Instantiate(highScoreTilePrefab, content.transform);
            scoreTile.ShowScoreFromInfo(info.Value, i + 1);
            scoreTiles.Add(scoreTile);
        }
    }

    public void ResetAll()
    {
        if (scoreTiles.Count > 0)
        {
            int count = scoreTiles.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(scoreTiles[i].gameObject);
            }
            scoreTiles.Clear();
        }

        gameObject.SetActive(false);
    }

}
