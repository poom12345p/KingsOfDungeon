using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour {
    [Header("Base")]
    public int id = -1;
    public bool isDeath;
    public Animator anim;
    public AudioSource source;
    public AudioClip deadSound;
    public StatContol statContol;
    public UltimateManager ultimateManager;
    List<MsgReciver> msgReciversList = new List<MsgReciver>();
    [Header("Agent properties")]
    public float turnspeed;
    protected float angle;
    protected float x, y;

    // Use this for initialization
    void Start () {
        statContol = GetComponent<StatContol>();
    }

    // Update is called once per frame
 
    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }


    public StatContol getCharacterStat()
    {
        return statContol;
    }
    public UltimateManager GetUltiManager()
    {
        return ultimateManager;

    }

    public void setCharacterStat(StatContol newStat)
    {
        statContol = newStat;
    }

    public void ReportUltiFull()
    {
        GameManagerPC.Instance.sendMsgToController(id, "ULTIFULL");
    }
    public void ReportToClient(string msg)
    {
        GameManagerPC.Instance.sendMsgToController(id, msg);
    }

    public void AddMsgReciver(MsgReciver msgr)
    {
        if(!msgReciversList.Contains(msgr))
        {
            msgReciversList.Add(msgr);
        }
    }
    public void removeMsgReciver(MsgReciver msgr)
    {
        if (msgReciversList.Contains(msgr))
        {
            msgReciversList.Remove(msgr);
        }
    }

    virtual public void getCommand(string cmd)
    {
        foreach (var msgr in msgReciversList)
        {
            msgr.reciveMsg(cmd);
        }
    }
    virtual public void getUpSkill(int num)
    {

    }
    public void sendMsgToClient(string msg)
    {
        GameManagerPC.Instance.sendMsgToController(id, msg);
    }
}
