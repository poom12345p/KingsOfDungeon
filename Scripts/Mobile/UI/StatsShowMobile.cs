using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsShowMobile : MonoBehaviour
{

    public Slider hpBar;

    public GameObject statPanel;

    [Header("StatText")]
    public Text hpStat;
    public Text attackStat;
    public Text speedStat;

    public ShowGetSkillScrollView skillScrollView;

    private void Start()
    {
        statPanel.SetActive(false);
        if (GameManagerMobile.Instance != null) GameManagerMobile.Instance.setStateShow(this);
    }

    public void TriggerShowStat()
    {
        statPanel.gameObject.SetActive(!statPanel.activeInHierarchy);
    }

    public void OnStatChange(PlayerStatMobile stat)
    {
        hpStat.text = "HP: " + stat.currentHp + "/" + stat.maxHp;
        hpBar.maxValue = stat.maxHp;
        hpBar.value = stat.currentHp;

        attackStat.text = "Attack: " + stat.attack;

        speedStat.text = "Speed: " + stat.speed;
    }

    public void OnUpGradeSkill(Dictionary<SkillInfo, int> accuiredSkill)
    {
        skillScrollView.SetSkillWithDictionary(accuiredSkill);
    }

    private void OnDisable()
    {
        skillScrollView.ClearSkill();
    }
}

public struct PlayerStatMobile
{
    public int maxHp;
    public int currentHp;
    public int attack;
    public float speed;

    public PlayerStatMobile(int maxHp, int currentHp, int attack, float speed)
    {
        this.maxHp = maxHp;
        this.currentHp = currentHp;
        this.attack = attack;
        this.speed = speed;
    }
}
