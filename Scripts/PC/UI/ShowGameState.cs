using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGameState : MonoBehaviour
{
    public Text gameState;
    public Text[] HintDialogText;

    public GameObject hintWhenClearState;
    Animator animator;
    int animationPlayed = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animationPlayed = 0;
        PlayAnimation();
    }

    void PlayAnimation()
    {
        if (animationPlayed < HintDialogText.Length)
        {
            ResetHintDialogText();
            animator.SetTrigger("Fade");
            ++animationPlayed;
        }
    }

    void ResetHintDialogText()
    {
        for (int i = 0; i < HintDialogText.Length; i++)
        {
            HintDialogText[i].gameObject.SetActive(false);
        }

        HintDialogText[animationPlayed].gameObject.SetActive(true);
    }


    public void GameStateChange(int level)
    {
        gameState.text = "Stage " + level;
    }

    public void KingStateShow()
    {
        gameState.text = "King State";
    }

    public void TriggerHintClearState()
    {
        animator.SetTrigger("StateClear");
    }
}
