using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingStateManager : MonoBehaviour
{
    [System.Serializable]
    public struct ShadowCharacter
    {
        public string CharacterTitle;
        public unitHitbox characterUnit;
    }

    public ShadowCharacter[] shadowCharacters;

    public List<unitHitbox> generatedShadow;

    public WarpGateSystem warpGateToNextState;

    public nameTagControl shadowTagControl;


    // Use this for initialization
    void Start()
    {
        GameManagerPC gameManager = GameManagerPC.Instance;

        if (gameManager != null)
        {
            var teamSavedList = gameManager.saveManager.playerRank.GetHighestRankTeamInState(gameManager.mapLevel);
            GenerateShadow(teamSavedList);
        }

        if (warpGateToNextState == null) Debug.LogError("WarpGate To NextState is null");
        else warpGateToNextState.gameObject.SetActive(false);
    }

    private void AttachItemToShadow(unitHitbox unit, List<KeyValueSerializable<int, int>> itemOwned)
    {
        if (itemOwned != null)
        {
            foreach (var itemIdAmount in itemOwned)
            {
                var itemClone = GameManagerPC.Instance.gameAssetStorage.FindItemBase(itemIdAmount.key);
                for (int i = 0; i < itemIdAmount.value; i++)
                {
                    itemClone.doActionAPI(unit);
                }
            }
        }
    }

    private void UpSkillShadow(unitHitbox unit, List<KeyValueSerializable<int, int>> skillAccuired)
    {
        if (skillAccuired != null)
        {
            foreach (var skill in skillAccuired)
            {
                for (int i = 0; i < skill.value; i++)
                {
                    var unitControl = unit.GetComponent<CharecterControl>();
                    unitControl.getUpSkill(skill.key);
                }
            }
        }
    }

    private unitHitbox GetShadownInstaceFromTitle(string title)
    {
        foreach (var shadow in shadowCharacters)
        {
            if (shadow.CharacterTitle.ToUpper() == title.ToUpper()) return shadow.characterUnit;
        }

        Debug.LogWarning("Can't find character with title " + title);

        return null;
    }

    private void GenerateShadow(List<PlayerSavedInfo> teamSaveList)
    {
        int tagIndex = 0;
        foreach (var saveList in teamSaveList)
        {
            var shadowUnit = Instantiate(GetShadownInstaceFromTitle(saveList.job), transform.position, Quaternion.identity);
            ItemOnPlayer itemOnPlayer = shadowUnit.gameObject.AddComponent<ItemOnPlayer>();
            PlayerInfo ShandowInfo = new PlayerInfo(
                saveList.playerUid,
                saveList.name,
                0,
                saveList.job
            );
            // set item
            itemOnPlayer.SetItemHolder(ShandowInfo);
            AttachItemToShadow(shadowUnit, saveList.itemOwned);

            // set skill
            UpSkillShadow(shadowUnit, saveList.acuiredSkill);

            // tracking shadow
            ShadowCharacterTracking trackingScript = shadowUnit.gameObject.AddComponent<ShadowCharacterTracking>();
            shadowTagControl.create(tagIndex, saveList.name);
            trackingScript.SetKingStateOwner(this, tagIndex, shadowTagControl);
            ++tagIndex;
            generatedShadow.Add(shadowUnit);
        }
    }

    public void NotifyShadowCharacterDead()
    {
        bool allIsDead = true;
        foreach (var shadow in generatedShadow)
        {
            var tracking = shadow.GetComponent<ShadowCharacterTracking>();
            if (!tracking.isDead)
            {
                allIsDead = false;
                break;
            }
        }

        if (allIsDead)
        {
            TriggerWarpGateNextState();
        }
    }

    public void TriggerWarpGateNextState()
    {
        // TODO: Do something else ??
        warpGateToNextState.gameObject.SetActive(true);
    }

}
