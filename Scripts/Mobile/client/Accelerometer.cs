using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Accelerometer : MonoBehaviour {
    public Text x, y,z,magnitude;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        x.text = Input.acceleration.x.ToString();
        y.text = Input.acceleration.y.ToString();
        z.text = Input.acceleration.z.ToString();
        magnitude.text = Input.acceleration.sqrMagnitude.ToString();
    }
}
