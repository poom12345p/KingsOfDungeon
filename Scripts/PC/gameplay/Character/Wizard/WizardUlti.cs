using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardUlti : MonoBehaviour {
    public damageHitbox basehitbox;
    public GameObject Skill;
    public float AreaSize = 20;
    public int MaxSpawn = 1000;
    public int NumSpawn = 1;
    public float Duration = 3;
    public float DropRate;
    public Vector3 Offset = Vector3.up;

    private float timeTemp;
    private float timeTempDuration;
    private int count = 0;
    public bool isRaining;


    void Start()
    {
        StartRain();
        timeTemp = Time.time;
    }

    void Spawn(Vector3 position)
    {
        if (Skill == null)
            return;

       GameObject newIce =Instantiate(Skill, position, Skill.transform.rotation);
       damageHitbox dh = newIce.GetComponent<damageHitbox>();
        dh.setDamage(basehitbox.damage);
    }

    public void StartRain()
    {
        isRaining = true;
        timeTempDuration = Time.time;
    }

    void Update()
    {
        if (isRaining)
        {
            if (count < MaxSpawn && Time.time < timeTempDuration + Duration)
            {
                if (Time.time > timeTemp + DropRate)
                {
                    timeTemp = Time.time;
                    for (int i = 0; i < NumSpawn; i++)
                    {
                        count += 1;
                        Spawn(this.transform.position + new Vector3(Random.Range(-AreaSize, AreaSize), 0, Random.Range(-AreaSize, AreaSize)) + Offset);
                    }
                }
            }
            else
            {
                isRaining = false;
            }
        }
    }
    // Use this for initialization
   
}
