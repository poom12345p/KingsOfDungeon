using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotControl : MonoBehaviour {

	public Image itemImage;
	public TextMeshProUGUI amt;

	private ItemModel itemBase;

	public InventorySystem parent;

	public Sprite trans;

	private bool tapping;

	private Button button;


	private void Awake() {
		amt = GetComponentInChildren<TextMeshProUGUI>();
		amt.text = "";
		button = GetComponent<Button>();
		button.interactable = false;
	}

	public void SetItemSlot(ItemModel item, int stacked = 0)
	{
		if(item == null)
		{
			itemImage.sprite = trans;
			itemBase = null;
			amt.text = "";
			return;
		}
		else
		{
			itemImage.sprite = item.itemSprite;
			itemBase = item;
		}

		if(stacked > 0) amt.text = "x"+stacked;
	}

	public void ShowMoreInfo()
	{
		if(itemBase!=null)
			parent.ShowDescription(
				itemBase.itemName, 
				itemBase.itemDescription, 
				GetComponent<RectTransform>().position.x, 
				itemBase.usable
			);
	}

	public void HideInfo()
	{
		parent.HideDescription();
	}

}
