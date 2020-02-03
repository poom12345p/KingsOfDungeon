using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGetSkillScrollView : MonoBehaviour
{

    public GameObject content;

    public List<SkillGainShowUI> gainedSkillShowed = new List<SkillGainShowUI>();

    public SkillGainShowUI skillShowPrefab;

    public void SetSkillWithDictionary(Dictionary<SkillInfo, int> accuiredSkill)
    {
        ClearSkill();

        foreach (var skillGain in accuiredSkill)
        {
            SkillGainShowUI newShowSkill = Instantiate(skillShowPrefab, content.transform);
            newShowSkill.SetSkillInfo(skillGain.Key, skillGain.Value);
            gainedSkillShowed.Add(newShowSkill);
        }
    }

    public void ClearSkill()
    {
        int count = gainedSkillShowed.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(gainedSkillShowed[i].gameObject);
        }

        gainedSkillShowed.Clear();
    }

}
