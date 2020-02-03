using UnityEngine;

public class ShieldEffectControl : MonoBehaviour, itemEffect
{

    public unitHitbox owner;
    public float shieldHeight = 1f;

    public GameObject[] shieldChild;

    public int shieldCharge = 0;
    public int hpPerCharge = 50;
    public int currentHp = 0;
    public int maxHp = 0;

    public GameObject ShieldBreakBomb;
    public float damageAfterBreakPerMaxHp = 0.5f;
    public ItemBase fromItem;

    public bool isRemoved = false;

    public void SetOwnerInstance(unitHitbox player)
    {
        owner = player;
        transform.localPosition = new Vector3(0, shieldHeight, 0);
        GetMoreItem();
    }

    public void GetMoreItem()
    {
        shieldCharge++;
        // reset hp when get more item
        currentHp = maxHp = hpPerCharge * shieldCharge;
        ChangeShieldSize();
    }

    public void takeDamage(int damage, unitHitbox attacker)
    {

        currentHp -= damage;

        if (currentHp <= 0)
        {
            isRemoved = true;
            int realDamage = -currentHp;
            RemoveAllShield();
            owner.takenDamage(realDamage, attacker);
        }
        else
        {
            if (DamageTextControl.instance != null)
                DamageTextControl.instance.CreateDamageText(damage, owner.transform, Color.white);
        }

    }



    void ChangeShieldSize()
    {
        foreach (var shield in shieldChild)
        {
            shield.transform.localScale = Vector3.one * (0.4f + (Mathf.Clamp(shieldCharge, 0, 10) / 10f));
        }
    }

    public void RemoveEffect()
    {
        // if (shieldCharge < 0) return;
        shieldCharge--;

        if (shieldCharge <= 0)
        {
            GameObject shieldBreak = Instantiate(ShieldBreakBomb);
            shieldBreak.transform.position = owner.transform.position;
            var damageHb = shieldBreak.GetComponent<damageHitbox>();
            damageHb.setOwner(owner);
            damageHb.setDamage((int)(maxHp * damageAfterBreakPerMaxHp));
            Destroy(gameObject);
        }
    }

    void RemoveAllShield()
    {
        var itemOnPlayer = owner.GetComponent<ItemOnPlayer>();
        while (shieldCharge > 0 && itemOnPlayer != null)
        {
            itemOnPlayer.RemoveItem(fromItem);
        }
    }


}
