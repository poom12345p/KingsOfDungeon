using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerCounter : MonoBehaviour {
    public UnityEvent allPlayerInArea;
    public UnityEvent notFullArea;
    
    public int playerInArea=0;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerUp()
    {
        if (playerInArea  < GameManagerPC.Instance.playerManagement.connectedPlayer)
        {
            playerInArea++;
        }
        CheckUp();
    }

    public void PlayerDown()
    {
        CheckDown();
        if (playerInArea > 0)
        {
            playerInArea--;
        }
    }

    public void CheckUp()
    {
        if(playerInArea== GameManagerPC.Instance.playerManagement.connectedPlayer)
        {
            allPlayerInArea.Invoke();
        }
    }
    public void CheckDown()
    {
        if (playerInArea == GameManagerPC.Instance.playerManagement.connectedPlayer)
        {
            notFullArea.Invoke();
        }
    }



}
