using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New ItemModel", menuName="Item Model")]
public class ItemModel : ScriptableObject {

	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;

	public string popupWhenGetItemText;

	public bool usable = false;

}
