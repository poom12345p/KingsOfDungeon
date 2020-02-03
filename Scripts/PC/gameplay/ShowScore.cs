using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowScore : MonoBehaviour
{

    public PlayerScore[] playerScores;
    public GameObject ScoreBoard;
    public GameObject ScoreBoardContent;

    public HighScoreShow highScoreShow;

    private void Start()
    {
        ScoreBoardContent.SetActive(false);
    }

    void ShowPlayerContent()
    {
        GameManagerPC gm = GameManagerPC.Instance;
        if (gm == null) return;

        ScoreBoardContent.SetActive(true);
        ScoreBoardContent.GetComponent<CanvasGroup>().alpha = 0;

        for (int i = 0; i < PlayerManagement.maxPlayer; i++)
        {
            if (gm.playerManagement.playersInfo[i] != null)
            {
                playerScores[i].gameObject.SetActive(true);
                PlayerInfo player = gm.playerManagement.playersInfo[i];

                Sprite playerImage = GameManagerPC.Instance.gameAssetStorage.FindCharacterProtriatByTitle(player.job);
                playerScores[i].ShowInformation(playerImage, player.playerName, player.level, player.scoreTrack);
            }
            else
            {
                playerScores[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(FadeIn(ScoreBoardContent, null));
    }

    public void ShowScoresBoard()
    {
        ScoreBoard.SetActive(true);
        ScoreBoard.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(FadeIn(ScoreBoard, ShowPlayerContent));

    }

    IEnumerator FadeIn(GameObject screen, System.Action myDelegate)
    {
        while (screen.GetComponent<CanvasGroup>().alpha < 1)
        {
            yield return new WaitForSeconds(0.005f);
            screen.GetComponent<CanvasGroup>().alpha += 0.05f;
        }

        screen.GetComponent<CanvasGroup>().interactable = true;

        yield return new WaitForSeconds(0.75f);
        if (myDelegate != null)
            myDelegate();
    }

    public void ResetValue()
    {
        foreach (var score in playerScores)
        {
            score.gameObject.SetActive(false);
        }
        ScoreBoard.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown && ScoreBoard.activeInHierarchy)
        {
            ShowHighScoreBoard();
            ScoreBoard.SetActive(false);
        }
    }

    public void GoBackToMenu()
    {
        highScoreShow.ResetAll();
        if (GameManagerPC.Instance.state == GameManagerPC.GameState.Menu)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GameManagerPC.Instance.ResetValueAndGoToMenu();
        }
    }

    public void ShowHighScoreBoard()
    {
        gameObject.SetActive(true);
        highScoreShow.gameObject.SetActive(true);
        highScoreShow.ShowAllHighScore();
    }

    public void ToggleShowHighScore()
    {
        if (!highScoreShow.gameObject.activeInHierarchy) ShowHighScoreBoard();
        else GoBackToMenu();
    }
}
