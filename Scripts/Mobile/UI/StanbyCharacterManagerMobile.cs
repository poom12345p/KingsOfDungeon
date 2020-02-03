using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanbyCharacterManagerMobile : MonoBehaviour
{

    private Dictionary<string, GameObject> playerLook; //knight archer berserker warchief
    string currentJob = "KNIGHT";

    void Awake()
    {

        playerLook = new Dictionary<string, GameObject>();

        foreach (Transform children in transform)
        {
            playerLook.Add(children.name, children.gameObject);
            children.gameObject.SetActive(false);
        }

    }

    public void changeLook(string playerJob)
    {
        playerLook[currentJob].SetActive(false);

        if (playerLook.ContainsKey(playerJob))
        {
            playerLook[playerJob].SetActive(true);
            currentJob = playerJob;
        }
        else
        {
            Debug.LogWarning("No Job name for: " + playerJob);
        }

    }
}
