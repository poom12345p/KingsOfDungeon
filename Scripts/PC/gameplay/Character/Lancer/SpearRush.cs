using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpearRush : MonoBehaviour {
    public List<unitHitbox> targets=new List<unitHitbox>();
    public int maxtarget;
    public string targetTag;

    // Use this for initialization
    void Start()
    {
        targets.Clear();
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targets.Count>0)
        {
            Vector3 pos = transform.position;
            //pos.y = 0;
            var deadtargets = new List<unitHitbox>();
            foreach (var target in targets)
            {
                if (target)
                {
                    target.transform.position = pos;
                    if (target.isDeath) deadtargets.Add(target);
                }
                
            }

            foreach (var target in deadtargets)
            {
                targets.Remove(target);
            }
            
        }
      

    }
    private void OnTriggerEnter(Collider other)
    {
        if (targets.Count < maxtarget)
        {
            if (other.tag == targetTag )
            {
                var temptarget = other.GetComponent<unitHitbox>();
                if (temptarget != null && !temptarget.isDeath && temptarget.statContol !=null && !targets.Contains(temptarget))
                {
                    targets.Add(temptarget);
                    targets[targets.Count-1].statContol.ChangeSpeedPercent(-1f);
                    Animator taranim = targets[targets.Count - 1].GetComponent<Animator>();
                    if(taranim !=null)taranim.speed = 0;
          
                }
            }
        }
    }

    public void reset()
    {
        foreach (var target in targets)
        {
            if (target != null)
            {
                if (target.statContol != null)
                {
                    target.statContol.ChangeSpeedPercent(1f);
                    targets[targets.Count - 1].statContol.ChangeSpeedPercent(-1f);
                }
                Animator taranim = target.GetComponent<Animator>();
                if (taranim != null) taranim.speed = 1;
            }

        }
        targets.Clear();
    }



}
