using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTotemControl : MonoBehaviour, EnemyControl
{

    public GameObject healArtea;
    public StatContol stat;
    // Use this for initialization
    void Start () {
        stat = GetComponent<StatContol>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ReportDeath()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        healArtea.SetActive(false);
       
        Invoke("selfDestruction", 0.5f);
    }

    public void selfDestruction()
    {
        Debug.Log("Destory");
        Destroy(gameObject);
    }

    public StatContol getCharacterStat()
    {
        return stat;
    }
}
