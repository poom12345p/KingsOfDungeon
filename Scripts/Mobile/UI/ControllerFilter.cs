using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ControllerFilter : MonoBehaviour {
    public GameObject  knight, archer;
    public GameObject[] allController;
    public Text nameText;
    public Image iconImg;
    public Sprite[] iconList;
    public UnityEvent MapInteract, ChestInteract, EndInteract;

    // Use this for initialization
    void Start () {
		if(GameManagerMobile.Instance.playerInfo !=null)
        {
            foreach (var con in allController)
            {
                con.SetActive(false);
            }
            switch(GameManagerMobile.Instance.playerInfo.job)
            {
                case "KNIGHT":
                    knight.SetActive(true);
                    GameManagerMobile.Instance.myController = knight.GetComponent<Controller>();
                    break;
                case "ARCHER":
                    archer.SetActive(true);
                    GameManagerMobile.Instance.myController = archer.GetComponent<Controller>();
                    break;
                case "WIZARD":
                    allController[2].SetActive(true);
                    GameManagerMobile.Instance.myController = allController[2].GetComponent<Controller>();
                    break;
                case "LANCER":
                    allController[4].SetActive(true);
                    GameManagerMobile.Instance.myController = allController[4].GetComponent<Controller>();
                    break;
            }
        }
        nameText.text = GameManagerMobile.Instance.playerInfo.playerName;

        iconImg.sprite = iconList[GameManagerMobile.Instance.playerNumber];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void clickBtn(GameObject controller)
    {
        foreach(GameObject ctlr in allController)
        {
            ctlr.SetActive(false);
        }
        controller.SetActive(true);
        GameManagerMobile.Instance.myController = controller.GetComponent<Controller>();
    }
}
