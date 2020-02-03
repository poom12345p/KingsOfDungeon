using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ItemsScrollList : MonoBehaviour {

    // name should be uniqe for checking
    private List<GameObject> itemList;
    // recycle
    public Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    private void Start()
    {
        itemList = new List<GameObject>();
    }

    public void AddItem(GameObject itemToAdd)
    {
        itemList.Add(itemToAdd);
    }

    public void RemoveAll()
    {
        itemList.Clear();
        for(int i=0; i< transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeInHierarchy)
            {
                inactiveInstances.Push(transform.GetChild(i).gameObject);
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowList()
    {
        foreach(GameObject item in itemList)
        {
           item.transform.SetParent(transform, false);
        }
    }
    public GameObject RecycleScrollList()
    {
        return inactiveInstances.Pop();
    }
}
