using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffArea : MonoBehaviour {
    public float lifetime;
    public float radius;
    [SerializeField] private List<EnemyAI> slaveList = new List<EnemyAI>();
    [Header("speed")]
    public bool isSpeedEffected = false;
    [Range(-1f, 1f)] public float speedEffectValue;
	// Use this for initialization
	void Start () {
        Invoke("clearEffect", lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        EnemyAI slave = collision.gameObject.GetComponent<EnemyAI>();
        if (slave != null)
        {
            if (!slaveList.Contains(slave))
            {
                slaveList.Add(slave);
                if (isSpeedEffected)
                {
                    slave.addSpeed(speedEffectValue);
                }
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
        
    //}
    private void OnTriggerExit(Collider other)
    {
        EnemyAI slave = other.gameObject.GetComponent<EnemyAI>();
        if(slave  != null)
        {
            if (slaveList.Contains(slave))
            {
                slaveList.Remove(slave);
                if (isSpeedEffected)
                {
                    slave.addSpeed(-speedEffectValue);
                }
            }
        }
    }

    public void doAction()
    {
        if(isSpeedEffected)
        {

        }
    }
    public void clearEffect()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        foreach(EnemyAI slave in slaveList)
        {
            if (isSpeedEffected)
            {
                slave.addSpeed(-speedEffectValue);
            }
        }
        slaveList.Clear();
    }
}
