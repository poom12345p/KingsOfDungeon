using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharStatUI : MonoBehaviour {
    [System.Serializable]
    public struct UIset
    {
        public string job;
        public GameObject statData;
        public GameObject btn;
    }
    public  UIset[] charUI;

    public Text classText;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setUI(string Job)
    {
        UIset myUI = getUIset(classText.text);
        myUI.statData.SetActive(false);
        myUI.btn.SetActive(false);
        classText.text = Job;
        myUI = getUIset(classText.text);
        myUI.statData.SetActive(true);
        myUI.btn.SetActive(true);

    }

    UIset getUIset(string job)
    {
        for (int i = 0; i <charUI.Length; i++)
        {
            if(charUI[i].job == job)
            {
                return charUI[i];
            }
        }
        return charUI[0];
    }
}
