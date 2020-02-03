using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenControl : MonoBehaviour
{

    public static LoadingScreenControl Instance;
    private AsyncOperation currentLoadingOperation;

    private bool isLoading;

    [SerializeField]
    private Text loadingText;

    [Header("HintShow")]
    [SerializeField]
    private Text hintText;
    [SerializeField]
    private List<string> hintList;
    private int formerHint = -1;

    Animator animator;

    private const float MIN_TIME_TO_SHOW = 1f;
    private float timeElapsed;

    private bool didTriggerFadeOutAnimation;

    private string mapToLoad = "";
    private bool alreadyTriggerLoading;

    private void Awake()
    {
        // Singleton logic:
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        // Hide();
    }
    private void Update()
    {
        if (isLoading)
        {
            SetProgress(currentLoadingOperation.progress);
            if (currentLoadingOperation.isDone && !didTriggerFadeOutAnimation)
            {
                isLoading = false;
                animator.SetTrigger("FadeOut");
                didTriggerFadeOutAnimation = true;
            }
            else
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= MIN_TIME_TO_SHOW)
                {
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }
        }
    }
    // Updates the UI based on the progress:
    private void SetProgress(float progress)
    {
        loadingText.text = "Loading " + Mathf.CeilToInt(progress * 100).ToString() + "%";
    }

    public void ShowLoading(string map)
    {
        // Enable the loading screen:
        if (!alreadyTriggerLoading)
        {
            ShowHintRandom();
            alreadyTriggerLoading = true;
            gameObject.SetActive(true);
            SetProgress(0f);
            timeElapsed = 0f;
            didTriggerFadeOutAnimation = false;
            mapToLoad = map;
            animator.SetTrigger("FadeIn");
        }

    }

    void Loading(AsyncOperation loadingOperation)
    {
        // Store the reference:
        currentLoadingOperation = loadingOperation;
        currentLoadingOperation.allowSceneActivation = false;
    }

    public void FinishLoading()
    {
        Hide();
        if (GameManagerPC.Instance != null)
            GameManagerPC.Instance.AfterFinishLoading();
    }

    // Call this to hide it:
    public void Hide()
    {
        // Disable the loading screen:
        alreadyTriggerLoading = false;
        mapToLoad = "";
        SetProgress(1f);
        currentLoadingOperation = null;
        gameObject.SetActive(false);
        //StopCoroutine(showHintCoroutine);
    }

    // use when fadeIn Animation complete
    public void StartLoadingScene()
    {
        if (mapToLoad != "")
        {
            isLoading = true;
            // TriggerShowHint();
            Loading(SceneManager.LoadSceneAsync(mapToLoad));
        }
        else
            Debug.LogError("Can't load blank string ");
    }

    public bool IsLoadingScene()
    {
        return isLoading;
    }

    private void ShowHintRandom()
    {
        if (hintText != null)
        {
            int hintChoice = formerHint;
            while (hintChoice == formerHint)
            {
                hintChoice = Random.Range(0, hintList.Count);
                if (hintList.Count == 1) break;
            }
            hintText.text = hintList[hintChoice];
            formerHint = hintChoice;
        }
    }

    // public void TriggerShowHint()
    // {
    //     if (isLoading)
    //         animator.SetTrigger("HintChange");
    // }
}
