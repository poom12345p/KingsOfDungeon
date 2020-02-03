using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCharacterTracking : MonoBehaviour
{

    public bool isDead = false;

    private KingStateManager kingstate;

    public int nameTagindex = -1;

    public nameTagControl shadowTagControl;

    public void SetKingStateOwner(KingStateManager kingstate, int tagIndex, nameTagControl shadowTagControl)
    {
        isDead = false;
        this.kingstate = kingstate;
        nameTagindex = tagIndex;

        this.shadowTagControl = shadowTagControl;
    }

    public void NotifyDead()
    {
        isDead = true;
        ItemOnPlayer itemOnPlayer = GetComponent<ItemOnPlayer>();
        if (itemOnPlayer != null)
        {
            itemOnPlayer.RemoveAllItem();
        }
        kingstate.NotifyShadowCharacterDead();
    }

    private void Update()
    {
        if (shadowTagControl != null && nameTagindex >= 0)
            shadowTagControl.setNameTagToPlayer(nameTagindex, transform);
    }
}
