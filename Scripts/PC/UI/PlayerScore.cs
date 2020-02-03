using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{

    public Image characterImage;
    public Text nameShow;
    // public Text levelShow;

    public Text[] scoreText; // 0: monsterKilled, 1: itemOwned: , 2: mapPassed
    ScoreTrack scoreTrack;

    public Text TotalScore;

    void SetScore()
    {
        SetScoreText(0, "Monster Killed: " + scoreTrack.monsterKilled + "*10");
        SetScoreText(1, "Item Owned: " + scoreTrack.itemOwned + "*2");
        SetScoreText(2, "Map Pass: " + scoreTrack.mapPassed + "*1000");
    }

    void SetScoreText(int index, string showScore)
    {
        scoreText[index].transform.parent.gameObject.SetActive(true);
        scoreText[index].text = showScore;
    }

    public void ShowInformation(Sprite character, string name, int level, ScoreTrack score)
    {
        characterImage.sprite = character;
        nameShow.text = "Name: " + name;
        //levelShow.text = "LV: " + level;

        scoreTrack = score;
        SetScore();
        TotalScore.text = "TotalScore :" + score.CalculateScoreValue();
    }

}
