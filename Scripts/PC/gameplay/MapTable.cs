using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTable : MonoBehaviour
{
    public PlayerCounter pc;
    public InteractArea intA;
    public GameObject displayText;
    public Text countText;
    bool isPlayerTrigger;
   [SerializeField] int playerOnTable;
    // Use this for initialization
    void Start()
    {
        if (displayText != null)
            displayText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayersIn()
    {
        if (!isPlayerTrigger && playerOnTable == 0)
        {
            if(displayText)displayText.SetActive(true);
            isPlayerTrigger = true;
        }
        playerOnTable++;
        updatePlayer();
    }

    public void PlayersOut()
    {
        playerOnTable--;
        updatePlayer(); 
        if (isPlayerTrigger && playerOnTable == 0)
        {
            if (displayText)displayText.SetActive(false);
            isPlayerTrigger = false;
        }
        checkInterract();



    }

    public void updatePlayer()
    {
        if (isPlayerTrigger)
        {
            countText.text = pc.playerInArea + "/" + GameManagerPC.Instance.playerManagement.connectedPlayer;
        }
    }

    public void checkInterract()
    {
        Debug.Log(pc.playerInArea+"|" +playerOnTable);
        if (pc.playerInArea >playerOnTable)
        {
            pc.PlayerDown();
        }
    }
}
