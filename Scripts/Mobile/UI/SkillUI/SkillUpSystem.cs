using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpSystem : MonoBehaviour
{

    public CoinSystem coinSystem;
    public SkillUpShow skillUpShow;

    private Dictionary<SkillInfo, int> accuiredSkill = new Dictionary<SkillInfo, int>();

    List<SkillInfo> allSkills = new List<SkillInfo>();
    public SkillInfo[] skillAvailable = new SkillInfo[3];
    public int coinNeededToUpSkill = 10;
    public int coinUpPerSkill = 50;
    public StatsShowMobile statsShow;

    [Header("Button")]
    public Button skillShowButton;
    private Color normalSkillButtonColor;
    [SerializeField]
    private Color canUpSkillButtonColor;

    [System.Serializable]
    public struct SkillRarityChance
    {
        public SkillInfo.SkillTier rarity;
        public int chance;

    }

    [Header("SkillChance")]
    public List<SkillRarityChance> skillChances;
    public List<SkillInfo> randomSkillChanceList;

    private void Awake()
    {
        normalSkillButtonColor = skillShowButton.colors.normalColor;
    }


    private void Start()
    {
        skillUpShow.gameObject.SetActive(false);
        if (GameManagerMobile.Instance != null && GameManagerMobile.Instance.gameAsset != null)
        {
            allSkills = GameManagerMobile.Instance.gameAsset.GetSkillInfoForCharacter(GameManagerMobile.Instance.myJob);
            // generate random skill Chance
            var generatedSkillChance = new List<SkillInfo>();
            foreach (var skill in allSkills)
            {
                foreach (var chance in skillChances)
                {
                    if (skill.tier == chance.rarity)
                    {
                        for (int i = 0; i < chance.chance; i++)
                        {
                            generatedSkillChance.Add(skill);
                        }

                        break;
                    }
                }
            }
            randomSkillChanceList = generatedSkillChance;
            GetRandomSkillSet();
        }
    }

    public void TriggerOpenOrCloseSkillPanle()
    {
        // TODO: Add some animation later
        if (skillUpShow.gameObject.activeInHierarchy)
        {
            skillUpShow.gameObject.SetActive(false);
        }
        else
        {
            SkillSetShow();
        }
    }

    public void SkillSetShow()
    {
        skillUpShow.SetAllSkillPanel(skillAvailable, coinNeededToUpSkill);
        skillUpShow.SetButtonInteractable(coinSystem.coin >= coinNeededToUpSkill);
        skillUpShow.gameObject.SetActive(true);
    }

    public void GetRandomSkillSet()
    {
        // ssr:1 > sr:2 > r:4 > n:8
        System.Array.Clear(skillAvailable, 0, skillAvailable.Length);


        // Shuffle allSkillsForCharacter
        int count = randomSkillChanceList.Count;
        int last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = randomSkillChanceList[i];
            randomSkillChanceList[i] = randomSkillChanceList[r];
            randomSkillChanceList[r] = tmp;
        }

        // get first three valid skill
        // skillAvailable
        int j = 0;
        foreach (var skill in randomSkillChanceList)
        {
            // check requirement to get that skill
            // 1 not reach limit
            if (accuiredSkill.ContainsKey(skill) && accuiredSkill[skill] >= skill.limitAmount)
            {
                // already reach limit
                continue;
            }
            // 2 has needed skill
            else if (skill.neededSkill != null && !accuiredSkill.ContainsKey(skill.neededSkill))
            {
                // don't have needed skill yet
                continue;
            }
            // if skillAvailable already contain this skill skip
            bool alreadyContainSkill = false;
            foreach (var item in skillAvailable)
            {
                if (item != null && item.skillCode == skill.skillCode)
                {
                    alreadyContainSkill = true;
                    break;
                }
            }

            if (alreadyContainSkill) continue;

            skillAvailable[j] = skill;
            ++j;
            if (j >= skillAvailable.Length) break;
        }





        foreach (var entry in accuiredSkill)
        {
            print(entry.Key.skillTitle + ": " + entry.Value);
        }
    }

    public void Upskill(int skillCode)
    {

        SkillInfo skillInfo = null;
        foreach (var skill in allSkills)
        {
            if (skill.skillCode == skillCode)
            {
                skillInfo = skill;
                break;
            }
        }

        if (accuiredSkill.ContainsKey(skillInfo)) accuiredSkill[skillInfo] += 1;
        else accuiredSkill.Add(skillInfo, 1);


        GetRandomSkillSet();

        // Decrease coin per amount use to up skill
        int oldCoinNeeded = coinNeededToUpSkill;
        coinNeededToUpSkill += coinUpPerSkill;
        coinSystem.UpdateCoin(-oldCoinNeeded);
        SkillSetShow();

        TriggerOpenOrCloseSkillPanle();
        statsShow.OnUpGradeSkill(accuiredSkill);

    }

    public void ShuffleNewRandomSkillSet()
    {
        GetRandomSkillSet();
        // Decrease coin per amount use to up skill
        coinSystem.UpdateCoin(-(coinNeededToUpSkill / 2));
        SkillSetShow();
    }

    public void coinIsUpdated()
    {
        bool canUpSkill = coinSystem.coin >= coinNeededToUpSkill;
        skillUpShow.SetButtonInteractable(canUpSkill);
        skillUpShow.SetResetSkillButtonEnable(coinSystem.coin >= (coinNeededToUpSkill / 2));

        if (canUpSkill)
        {
            ChangeButtonColor(canUpSkillButtonColor);
        }
        else
        {
            ChangeButtonColor(normalSkillButtonColor);
        }

    }

    void ChangeButtonColor(Color newColor)
    {
        var colors = skillShowButton.colors;
        colors.normalColor = newColor;
        skillShowButton.colors = colors;
    }

}
