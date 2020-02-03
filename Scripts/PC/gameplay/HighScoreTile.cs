using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTile : MonoBehaviour
{

    public Text OrderText;
    public Text totalScoreText;
    public Text statePass;

    [System.Serializable]
    public struct CharacterInfoShow
    {
        public Text nameText;
        public Text characterText;
        public Text scoreText;
        public Image characterImage;
        public GameObject representObject;
    }

    public CharacterInfoShow[] charactersInfoShow;

    public void ShowScoreFromInfo(List<PlayerSavedInfo> info, int order)
    {
        OrderText.text = order.ToString();
        int sumScore = 0;
        for (int i = 0; i < charactersInfoShow.Length; i++)
        {
            if (i < info.Count)
            {
                charactersInfoShow[i].representObject.SetActive(true);
                charactersInfoShow[i].characterImage.sprite = GameManagerPC.Instance.gameAssetStorage.FindCharacterProtriatByTitle(info[i].job);
                charactersInfoShow[i].nameText.text = info[i].name;
                charactersInfoShow[i].characterText.text = info[i].job;

                int currentScore = info[i].score.CalculateScoreValue();
                charactersInfoShow[i].scoreText.text = currentScore.ToString();
                sumScore += currentScore;
            }
            else
            {
                charactersInfoShow[i].representObject.SetActive(false);
            }

        }

        totalScoreText.text = "Total Score: " + sumScore;
        statePass.text = "State: " + info[0].score.mapPassed;
    }
}
