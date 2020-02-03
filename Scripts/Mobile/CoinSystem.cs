using UnityEngine;
using UnityEngine.UI;

public class CoinSystem : MonoBehaviour
{

    public int coin;

    public Text ShowCoin;

    public SkillUpSystem skillUpSystem;

    private void Start()
    {
        coin = 0;
        UpdateCoin(0);
        if (GameManagerMobile.Instance != null)
            GameManagerMobile.Instance.coinSystem = this;
    }

    public void UpdateCoin(int amount)
    {
        coin += amount;
        ShowCoin.text = "X " + coin;

        skillUpSystem.coinIsUpdated();
    }


}
