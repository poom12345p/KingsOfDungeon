using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReviveArea : MonoBehaviour
{
    public unitHitbox myHitbox;
    int point = 0;
    const int downPoint = 1;
    const int maxPoint = 320;
    Collider myCol;
    CharecterControl charecterControl;
    [SerializeField] List<unitHitbox> playerInArea = new List<unitHitbox>();
    public GuageRing guageRing;
    public List<GameObject> subObj;
    public ParticleSystem reviveEff;
    // Use this for initialization
    void Start()
    {
        myCol = gameObject.GetComponent<Collider>();
        //  activeReviveArea();
        deActiveReviveArea();
    }

    IEnumerator reviving()
    {
        while (point < maxPoint)
        {
            //Debug.Log(gameObject.name + "revieve count ++");
            yield return new WaitForFixedUpdate();
            checkPlayer();
            if (playerInArea.Count > 0)
            {
                point += playerInArea.Count;
                if (reviveEff.isStopped) reviveEff.Play();
            }
            if (playerInArea.Count == 0 && point > 0)
            {
                point -= downPoint;
                if (reviveEff.isPlaying) reviveEff.Stop();
            }
            guageRing.updateGauge(maxPoint, point);
            if (point >= maxPoint)
            {
                revive();
            }
            if (point < 0)
            {
                point = 0;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void activeReviveArea()
    {
        point = 0;
        playerInArea.Clear();
        foreach (GameObject obj in subObj)
            if (obj != null) obj.SetActive(true);
        myCol.enabled = true;
        //particle.SetActive(true);
        guageRing.updateGaugeImediate(maxPoint, 0);
        StartCoroutine("reviving");

    }
    public void deActiveReviveArea()
    {
        foreach (GameObject obj in subObj)
            if (obj != null) obj.SetActive(false);
        myCol.enabled = false;
        StopCoroutine("reviving");
        playerInArea.Clear();
        // particle.SetActive(false);

    }
    void revive()
    {
        point = 0;
        playerInArea.Clear();
        myHitbox.reviveAction();

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            // Debug.Log(other);
            var otherhitbox = other.GetComponent<unitHitbox>();
            if (otherhitbox.isDeath) return;
            if (!playerInArea.Contains(otherhitbox) && otherhitbox != myHitbox) playerInArea.Add(otherhitbox);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log(other);
            var otherhitbox = other.GetComponent<unitHitbox>();
            if (playerInArea.Contains(otherhitbox)) playerInArea.Remove(otherhitbox);
        }
    }

    public void checkPlayer()
    {
        for (int i = 0; i < playerInArea.Count; i++)
        {
            if (playerInArea.Contains(playerInArea[i]) && playerInArea[i].isDeath && playerInArea.Count != 0)
            {
                playerInArea.Remove(playerInArea[i]);
                i--;
            }

        }

    }



}
