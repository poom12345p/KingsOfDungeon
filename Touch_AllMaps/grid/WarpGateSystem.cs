using UnityEngine;

public class WarpGateSystem : MonoBehaviour
{

    private bool isAlreadyLoading = false;

    public void warp()
    {
        isAlreadyLoading = true;
        if (GameManagerPC.Instance.state == GameManagerPC.GameState.GamePlay)
            GameManagerPC.Instance.LoadNextState();
        else if (GameManagerPC.Instance.state == GameManagerPC.GameState.TutorialPlay)
            GameManagerPC.Instance.ResetValueAndGoToMenu();
        else
            Debug.LogError("Unhandled WarpGateSystem state");
    }

    public void GoNextLevelAfterClearKingState()
    {
        GameManagerPC.Instance.LoadNextRandomMap();
    }

}
