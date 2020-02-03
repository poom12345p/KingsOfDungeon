using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropControl : MonoBehaviour {

    [SerializeField]int maxDorpItem;
    [SerializeField] int maxDorpPotion;
    [SerializeField] int DropCoinAmount;
    const float dropRange = 3.0f;
    [SerializeField]
	private int sumRatePotion=0,sumRateItem=0;
    [System.Serializable]
    public struct dropData
    {
        public GameObject item;
        public int rate;
    }

    public dropData[] potionsDropData;
    public dropData[]   itemsDropData;

    public GameObject coinPrefab;

    void Start () {

        sumRatePotion = sumRateItem = 0;
        foreach (dropData drop in itemsDropData)
        {
            sumRateItem += drop.rate;
        }
        foreach (dropData drop in potionsDropData)
        {
            sumRatePotion += drop.rate;
            // Debug.Log("sumrate potion += " + drop.rate);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void dropItem()
    {

    }

    public void randomDrop()
    {
        List<GameObject> dropsList= new List<GameObject>();
        for (int j = 0; j < GameManagerPC.Instance.playerManagement.connectedPlayer; j++)
        {
            for (int i = 0; i < maxDorpPotion; i++)
            {
                int k = Random.Range(1, sumRatePotion + 1);
                int sum = 0;
                foreach (dropData drop in potionsDropData)
                {
                    sum += drop.rate;
                    if (sum >= k)
                    {
                        if (drop.item != null)
                        {
                            dropsList.Add(drop.item);
                         
                        }
                        break;
                    }
                }
            }
            for (int i = 0; i < maxDorpItem; i++)
            {
                int k = Random.Range(1, sumRateItem + 1);
                int sum = 0;
                foreach (dropData drop in itemsDropData)
                {
                    sum += drop.rate;
                    if (sum >= k)
                    {
                        if (drop.item != null)
                        {
                            dropsList.Add(drop.item);
                           
                        }
                        break;
                    }
                }
            }
        }
      

        if (dropsList.Count > 0)
            createDrop(dropsList);
    }

    public void createDrop(List<GameObject> drops)
    {
        if (GameManagerPC.Instance == null)
            return;

       
            foreach (GameObject drop in drops)
            {
                Droping(drop);
            }
            for (int i = 0; i < DropCoinAmount * GameManagerPC.Instance.playerManagement.connectedPlayer; i++)
            {
                Droping(coinPrefab);
            }


    }

    public void Droping(GameObject thing)
    {
        GameObject drop = Instantiate(thing, transform.position, Quaternion.identity);
        Vector3 pos = transform.position;
        Vector3 destination = new Vector3(pos.x + Random.Range(-dropRange, dropRange), transform.position.y, pos.z + Random.Range(-dropRange, dropRange));
        DropItem dropItem = drop.GetComponent<DropItem>();
        dropItem.dropMove(destination);
    }
}

