using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTrigger : MonoBehaviour,MsgReciver {

    [SerializeField]BaseCharacter baseChar;
    public List<InteractArea> interactsList = new List<InteractArea>();
    public InteractArea interactArea;
    // Use this for initialization
    void Start () {
        baseChar.AddMsgReciver(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddInteract(InteractArea interactArea)
    {
        if(!interactsList.Contains(interactArea))
        {
            interactsList.Add(interactArea);
        }
        setArea();
    }


    public void RemoveInteract(InteractArea interactArea)
    {
        if (interactsList.Contains(interactArea))
        {
            interactsList.Remove(interactArea);
        }
        checkArea();
    }
    public void reciveMsg(string msg)
    {
        InteractType IT = InteractType.NONE;
        switch(msg)
        {
            case "SHAKEINT":
                IT = InteractType.SHAKE;
                break;
            case "UPINT":
                IT = InteractType.UP;
                break;
            case "DOWNINT":
                IT = InteractType.DOWN;
                break;
        }
        if(interactArea) interactArea.GetInteract(IT);
    }

    public void setArea()
    {
        if (interactsList.Count > 0 && interactArea == null)
        {
            interactArea = interactsList[0];
            baseChar.sendMsgToClient("ININTAREA");
            baseChar.sendMsgToClient(interactArea.areaType);
        }
    }

    public void checkArea()
    {
        if (interactsList.Count>0 && interactArea != interactsList[0])
        {
            interactArea = interactsList[0];
        }
        else if(interactsList.Count == 0)
        {
            interactArea = null;
            baseChar.sendMsgToClient("OUTINTAREA");
        }
        
    }
}
