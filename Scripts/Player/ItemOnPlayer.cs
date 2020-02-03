using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnPlayer : MonoBehaviour
{

    private PlayerInfo playerInfo;
    unitHitbox player;

    private void Start()
    {
        player = GetComponent<unitHitbox>();
        if (player == null) Debug.LogError("Can't find unithitbox on this instance");
    }

    public void SetItemHolder(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
    }

    public void AddItem(ItemBase item)
    {
        ItemBase clone = GameManagerPC.Instance.gameAssetStorage.GetItemClone(item);

        if (playerInfo.ItemHold.ContainsKey(clone)) playerInfo.ItemHold[clone] += 1;
        else playerInfo.ItemHold.Add(clone, 1);
    }

    public void RemoveItem(ItemBase item)
    {
        ItemBase clone = GameManagerPC.Instance.gameAssetStorage.GetItemClone(item);
        try
        {
            playerInfo.ItemHold[clone] -= 1;
            clone.removeItemEffectOnPlayer(player);
            GameManagerPC manager = GameManagerPC.Instance;
            if (manager != null)
            {
                manager.playerManagement.RemoveItemScore(player.id);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("No Item");
        }
        finally
        {
            if (playerInfo.ItemHold[clone] == 0)
            {
                if (!playerInfo.ItemHold.Remove(clone))
                {
                    Debug.LogError("Can't delete " + clone);
                }
            }
        }


        //DamageTextControl dmc = DamageTextControl.instance;
        //if(dmc != null) dmc.CreateGetItemText(clone.itemModel.itemName+" is Broken!", transform);

    }

    public void RemoveRandomItemOnPlayer(int amount)
    {
        List<ItemBase> itemsList = GetItemList();
        try
        {
            for (int i = 0; i < amount && CountItemOnPlayer() > 0; i++)
            {
                int removeIndex = Random.Range(0, itemsList.Count);
                RemoveItem(itemsList[removeIndex]);
                itemsList.RemoveAt(removeIndex);
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogError("Item Index out of range can't remove: " + e.Message);
        }
    }

    public int CountItemOnPlayer()
    {
        int count = 0;
        foreach (var item in playerInfo.ItemHold.Keys)
        {
            if (item.itemModel.itemCode != 0) count += playerInfo.ItemHold[item];
        }
        return count;
    }

    public List<ItemBase> GetItemList()
    {
        List<ItemBase> itemsList = new List<ItemBase>();

        foreach (var item in playerInfo.ItemHold.Keys)
        {
            if (item.itemModel.itemCode == 0) continue;
            for (int i = 0; i < playerInfo.ItemHold[item]; i++) itemsList.Add(item);
        }

        return itemsList;
    }

    public void RemoveAllItem()
    {
        var itemList = GetItemList();
        foreach (var item in itemList)
        {
            RemoveItem(item);
        }
    }
}
