using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Create new skill")]
public class SkillInfo : ScriptableObject
{
    public int skillCode;
    public string skillTitle;
    public string SkillDescription;
    public Sprite SkillImage;
    public string skillFor;

    public enum SkillTier
    {
        n, r, sr, ssr
    }

    public SkillTier tier;
    public SkillInfo neededSkill;
    public int limitAmount;

    // override object.Equals
    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SkillInfo other = (SkillInfo)obj;

        return skillCode == other.skillCode && skillFor == other.skillFor;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return skillCode;
    }

    public static string SkillTierToString(SkillTier tier)
    {
        string rarity = "";
        switch (tier)
        {
            case SkillTier.n:
                rarity = "n";
                break;
            case SkillTier.r:
                rarity = "r";
                break;
            case SkillTier.sr:
                rarity = "sr";
                break;
            case SkillTier.ssr:
                rarity = "ssr";
                break;
        }

        return rarity;
    }
}
