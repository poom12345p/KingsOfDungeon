using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour, afterPickItem
{

    public ItemModel itemModel;
    public DropItem dropItem;

    public bool picked = false;

    // no one pick for 30 sec: destroy this
    public float delayDestory = 30f;
    public bool neverDestroy = false;

    private void Start()
    {
        //dropItem = GetComponent<DropItem>();
        dropItem.addAfterPickItems(this);

        // destroy it if no one pick this
        if (!neverDestroy)
            Destroy(gameObject, delayDestory);
    }

    public virtual void doActionAPI(unitHitbox unitHitbox)
    {
        unitHitbox.addAfterPickItem(this);
        ItemOnPlayer itemOnPlayer = unitHitbox.GetComponent<ItemOnPlayer>();
        if (itemOnPlayer != null) itemOnPlayer.AddItem(this);
        sendItemToClient(unitHitbox.id);
        if (GameManagerPC.Instance != null && itemModel.itemCode != 0)
        {
            GameManagerPC.Instance.playerManagement.AddItemScore(unitHitbox.id);
        }
    }

    public void sendItemToClient(int playerId)
    {
        if (GameManagerPC.Instance != null)
        {
            PlayerInfo playerInfo = GameManagerPC.Instance.playerManagement.GetPlayerByID(playerId);

            if (MyServer.Instance != null && itemModel != null)
            {
                MyServer.Instance.SendItemDataToClient(itemModel.itemCode, playerInfo);
            }
        }
    }

    public void sendRemoveThisItemToClient(int playerId)
    {
        if (GameManagerPC.Instance != null)
        {
            PlayerInfo playerInfo = GameManagerPC.Instance.playerManagement.GetPlayerByID(playerId);
            if (MyServer.Instance != null && itemModel != null)
            {
                MyServer.Instance.SendRemoveItemDataToClient(itemModel.itemCode, playerInfo);
            }
        }
    }

    // override on other item to have difference check
    public virtual bool checkPickItemCondition(unitHitbox picker)
    {
        return true;
    }

    public virtual void removeItemEffectOnPlayer(unitHitbox player)
    {
        if (player.id >= 0)
        {
            // Debug.Log("Remove " + gameObject + " on player");
            sendRemoveThisItemToClient(player.id);
        }
    }

    // FOR EQUALITY CHECKING AMONG ITEM USE ITEMCODE FROM IT'S MODEL
    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        // TODO: write your implementation of Equals() here
        return ((ItemBase)obj).itemModel.itemCode == itemModel.itemCode;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return itemModel.itemCode;
    }
}
