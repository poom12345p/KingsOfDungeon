using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetStorage : MonoBehaviour
{

    [Header("All character in the game")]
    public List<PlayerCharacterAsset> characterAssets;

    [Header("All Item in the game")]
    public List<ItemBase> allItem;
    public List<BasePotion> allPotion;

    [Header("All Skill In Game")]
    public List<SkillInfo> skillInfos;

    public GameObject FindCharacterByTitle(string title)
    {
        foreach (var character in characterAssets)
        {
            if (character.characterTitle.ToUpper() == title.ToUpper())
            {
                return character.characterInstance;
            }
        }

        Debug.LogWarning("Can't find character with title: " + title);
        return null;
    }

    public Sprite FindCharacterProtriatByTitle(string title)
    {
        foreach (var character in characterAssets)
        {
            if (character.characterTitle.ToUpper() == title.ToUpper())
            {
                return character.characterProtrait;
            }
        }

        Debug.LogWarning("Can't find character with title: " + title);
        return null;
    }

    public List<ItemModel> GetAllItemModelFromItem()
    {
        List<ItemModel> itemModelList = new List<ItemModel>();

        foreach (var item in allItem)
        {
            itemModelList.Add(item.itemModel);
        }

        return itemModelList;
    }

    public ItemBase GetItemClone(ItemBase itemBase)
    {
        return FindItemBase(itemBase.itemModel.itemCode);
    }

    public ItemBase FindItemBase(int itemId)
    {
        foreach (var item in allItem)
        {
            if (item.itemModel.itemCode == itemId) return item;
        }

        Debug.LogWarning("No item with Id: " + itemId);
        return null;
    }

    public List<SkillInfo> GetSkillInfoForCharacter(string jobTitle)
    {
        List<SkillInfo> skills = new List<SkillInfo>();

        foreach (var skill in skillInfos)
        {
            if (skill.skillFor == jobTitle || skill.skillFor == "ALL") skills.Add(skill);
        }

        return skills;
    }

    public ItemBase GetRandomItem()
    {
        int randomItemIndex = Random.Range(0, allItem.Count);
        return allItem[randomItemIndex];
    }

}

[System.Serializable]
public struct PlayerCharacterAsset
{
    public string characterTitle;
    public GameObject characterInstance;
    public Sprite characterProtrait;
}
