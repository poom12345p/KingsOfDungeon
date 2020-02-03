using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentControl : MonoBehaviour,EnemyControl {
    EnemyAI enemyAI;
   public EnemyAI myCrew;
    public float spawntime;
    bool isDeath;
    public int maxEnemy;
    EnemyAI[] myCrewArray;
    public StatContol stat;
    // Use this for initialization
    void Start () {
        myCrewArray = new EnemyAI[maxEnemy];
        stat = GetComponent<StatContol>();
        StartCoroutine("spwaningMonster");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator spwaningMonster()
    {
        while(spawntime>0)
        {
            yield return new WaitForSeconds(spawntime);
            for(int i=0;i<myCrewArray.Length;i++)
            {
                if(myCrewArray[i]== null)
                {
                    myCrewArray[i] = Instantiate(myCrew, transform.position,Quaternion.identity);
                    myCrewArray[i].gameObject.SetActive(true);
                    myCrewArray[i].transform.position += Vector3.forward * 3;
                    break;
                }
            }
        }
    }

    public void ReportDeath()
    {
       
        isDeath = true;
        selfDestruction();
    }

    public void selfDestruction()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);

    }

    public StatContol getCharacterStat()
    {
        return stat;
    }
}
