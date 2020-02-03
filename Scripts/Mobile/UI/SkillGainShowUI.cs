using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillGainShowUI : MonoBehaviour
{

    public Text accuiredText;
    public Text rarity;
    public Image skillImage;
    public Text description;

    public void SetSkillInfo(SkillInfo skill, int accuired)
    {
        accuiredText.text = "X " + accuired;

        rarity.text = SkillInfo.SkillTierToString(skill.tier);

        skillImage.sprite = skill.SkillImage;

        description.text = skill.SkillDescription;

    }
}
