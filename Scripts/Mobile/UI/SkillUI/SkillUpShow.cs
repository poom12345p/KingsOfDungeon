using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpShow : MonoBehaviour
{

    [System.Serializable]
    public struct SkillPanelInformation
    {
        public int skillCode;
        // public Text skillTitle;
        public Text skillDescriptionText;
        public Image skillImage;
        public Text coinNeeded;
        public Text rarity;
    }

    public SkillPanelInformation[] skillPanels;
    public SkillUpSystem skillUpSystem;

    public Button[] skillButton;

    [Header("Reskill")]
    public Button skillResetButton;

    public Text reskillCost;
    bool disableReset = false;
    bool noSkillLeft = false;

    public GameObject NoSkillLeftPanel;

    private void Start()
    {
        NoSkillLeftPanel.SetActive(false);
    }

    public void SetAllSkillPanel(SkillInfo[] skillInfos, int skillCost)
    {
        noSkillLeft = true;
        for (int i = 0; i < skillPanels.Length; i++)
        {
            if (skillInfos[i] == null)
            {
                skillButton[i].gameObject.SetActive(false);
                disableReset = true;
            }
            else
            {
                skillPanels[i].skillCode = skillInfos[i].skillCode;
                // skillPanels[i].skillTitle.text = skillInfos[i].skillTitle;
                skillPanels[i].skillDescriptionText.text = skillInfos[i].SkillDescription;
                skillPanels[i].skillImage.sprite = skillInfos[i].SkillImage;
                skillPanels[i].coinNeeded.text = skillCost + " coin";

                switch (skillInfos[i].tier)
                {
                    case SkillInfo.SkillTier.n:
                        skillPanels[i].rarity.text = "N";
                        break;
                    case SkillInfo.SkillTier.r:
                        skillPanels[i].rarity.text = "R";
                        break;
                    case SkillInfo.SkillTier.sr:
                        skillPanels[i].rarity.text = "SR";
                        break;
                    case SkillInfo.SkillTier.ssr:
                        skillPanels[i].rarity.text = "SSR";
                        break;
                    default:
                        break;
                }

                noSkillLeft = false;
            }

        }

        if (!disableReset)
            reskillCost.text = (skillCost / 2) + " coin";

        if (noSkillLeft)
        {
            NoSkillLeftPanel.SetActive(true);
        }
    }

    public void SentSkillUpFromButton(int buttonIndex)
    {
        try
        {
            if (MyClient.Instance != null && MyClient.Instance.IsConnectedToServer())
                MyClient.SendUpSkillMessage(skillPanels[buttonIndex].skillCode);
            Debug.Log("Sending skillcode " + skillPanels[buttonIndex].skillCode + " To Server");
        }
        catch (System.Exception)
        {
            Debug.LogError("Can't send message to server, Check internet connection");
        }
        skillUpSystem.Upskill(skillPanels[buttonIndex].skillCode);

    }

    public void SetButtonInteractable(bool enable)
    {
        for (int i = 0; i < skillButton.Length; i++)
        {
            if (skillPanels[i].skillCode != -1)
                skillButton[i].interactable = enable;
            else
                skillButton[i].interactable = false;
        }

    }

    public void SetResetSkillButtonEnable(bool enable)
    {
        skillResetButton.interactable = enable && !disableReset;
    }
}
