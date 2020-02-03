using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GuageRing : MonoBehaviour {
    Quaternion oringinRotation;
    public Image gaugeImage;
    [Range(0f,1f)]
    public float maxhpGaugue;
     float changeSpeed=0.4f;
    float currentGaugue;
    float realGaugue;
    // Use this for initialization
    void Start () {
        oringinRotation = transform.rotation;
     
	}
	
	// Update is called once per frame
	void FixedUpdate () {

      
        if (gaugeImage.fillAmount < realGaugue)
        {
            gaugeImage.fillAmount += changeSpeed * Time.fixedDeltaTime;
        }
        else if (gaugeImage.fillAmount > realGaugue )
        {
            gaugeImage.fillAmount -= changeSpeed * Time.fixedDeltaTime;
        }
        if(gaugeImage.fillAmount < realGaugue+ changeSpeed * Time.fixedDeltaTime && gaugeImage.fillAmount > realGaugue- changeSpeed * Time.fixedDeltaTime)
        {
            gaugeImage.fillAmount = realGaugue;
        }
    }
    private void LateUpdate()
    {
        transform.rotation = oringinRotation;
    }

    public void updateGauge(float max,float cur)
    {
        if (cur > 0)
        {
            realGaugue= ((float)cur / (float)max) * maxhpGaugue;
            // gaugeImage.fillAmount = ((float)cur/ (float)max ) * maxhpGaugue;
        }
        else
        {
            realGaugue = 0;
        }
    }

    public void updateGaugeImediate(float max, float cur)
    {
        if (cur > 0)
        {
            realGaugue = ((float)cur / (float)max) * maxhpGaugue;
            gaugeImage.fillAmount = realGaugue;
        }
        else
        {
            realGaugue = 0;
            gaugeImage.fillAmount = realGaugue;
        }
    }
}
