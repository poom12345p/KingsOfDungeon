using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour {

	public List<ItemModel> itemInstances;

	//public List<ItemModel> currentItems;
	public Dictionary<ItemModel, int> currentItemsStacked = new Dictionary<ItemModel, int>();

	public GameObject itemDescriptionPopUp;

	private ItemSlotControl[] itemSlots;

	public GameObject prevSlotArrow;
	public GameObject nextSlotArrow;

	public GameObject showButton;

	public int currentSlot = 0;

	private Animator animator;

	private void Awake() {
        animator = gameObject.GetComponent<Animator>();
		itemSlots = GetComponentsInChildren<ItemSlotControl>();
		foreach (ItemSlotControl item in itemSlots)
		{
			item.parent = this;
		}
	}

	void Start()
	{
		if(GameManagerMobile.Instance != null) 
		{
			GameManagerMobile.Instance.inventorySystem = this;
			itemInstances = GameManagerMobile.Instance.gameAsset.GetAllItemModelFromItem();
		}
		EmptyAllSlot();
		RenderItemSlot();
		HideDescription();
		animator = GetComponent<Animator>();	
	}

	public void ShowInventory()
	{
		showButton.SetActive(false);
		RenderItemSlot();
		animator.SetBool("show", true);
	}

	public void HideInventory()
	{
		showButton.SetActive(true);
		animator.SetBool("show", false);
	}

	
	public void AddItem(int itemCode)
	{
		foreach(ItemModel im in itemInstances)
		{
			if(im.itemCode == itemCode){

				if(currentItemsStacked.ContainsKey(im)){
					currentItemsStacked[im] += 1;
				}
				else
				{
					currentItemsStacked.Add(im, 1);
				}

				RenderItemSlot();
				return;
			}
		}
	}

	public void RemoveItem(int itemCode){
		ItemModel removedItem = null;

		foreach(ItemModel im in currentItemsStacked.Keys)
		{
			if(im.itemCode == itemCode){
				currentItemsStacked[im] -= 1;
				removedItem = im;
				break;
			}
		}

		if(removedItem != null && currentItemsStacked[removedItem] == 0){
			currentItemsStacked.Remove(removedItem);
			EmptyAllSlot();
		}
		RenderItemSlot();		
	}

	private void RenderItemSlot()
	{
		int allSlots = itemSlots.Length;

		int j = 0;
		foreach (ItemModel item in currentItemsStacked.Keys)
		{
			int slot =  j-(currentSlot*allSlots);
			itemSlots[slot].SetItemSlot(item, currentItemsStacked[item]);
			j++;
		}
		
		RenderArrow();
	}

	private void RenderArrow()
	{
		if(currentSlot == 0) prevSlotArrow.SetActive(false);
		else prevSlotArrow.SetActive(true);

		if((currentSlot+1)*itemSlots.Length < currentItemsStacked.Keys.Count) nextSlotArrow.SetActive(true);
		else nextSlotArrow.SetActive(false);
	}

	private void EmptyAllSlot()
	{
		foreach (ItemSlotControl item in itemSlots)
		{
			item.SetItemSlot(null);
		}
	}

	public void ShowDescription(string name, string description, float positionX, bool usable)
    {
		itemDescriptionPopUp.gameObject.SetActive(true);
        TextMeshProUGUI[] popUp = itemDescriptionPopUp.GetComponentsInChildren<TextMeshProUGUI>();
		popUp[0].text = name;
		popUp[1].text = description;
		
		Vector3 currentPos = itemDescriptionPopUp.GetComponent<RectTransform>().position;
		itemDescriptionPopUp.GetComponent<RectTransform>().position = new Vector3(
			15+positionX,
			currentPos.y,
			currentPos.z
		);
    }

	public void HideDescription()
	{
		itemDescriptionPopUp.gameObject.SetActive(false);
	}

	public void ChangeSlot(int next){
		currentSlot += next;
		EmptyAllSlot();
		RenderItemSlot();
	}
}
