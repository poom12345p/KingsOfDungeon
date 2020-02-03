using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatFire : MonoBehaviour
{
    public Transform spot;
    public damageHitbox damHitbox;
    public float repeatTime, destoryIn;
    // Use this for initialization
    void Start()
    {
        if (destoryIn > 0) Destroy(gameObject, destoryIn);
        if (repeatTime > 0) StartCoroutine("repeating");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator repeating()
    {
        while (true)
        {
            // Debug.Log("spawn");
            yield return new WaitForSeconds(repeatTime);
            damHitbox.spawnDuplicate(spot);
        }
    }
}
