
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityStandardAssets.CrossPlatformInput;
using System.Net;
using System.Net.Sockets;
using Random = UnityEngine.Random;

public class NetworkServerUI : MonoBehaviour {

    CrossPlatformInputManager.VirtualAxis[] HRZTAxis = new CrossPlatformInputManager.VirtualAxis[4], 
                                            VTCAxis = new CrossPlatformInputManager.VirtualAxis[4];

    string hrztaxis_n = "Horizontal_", vtcaxis_n = "Vertical_";



    public PlayersCommander playersComand;
    public static string ipaddress;
    int playersCount = 0;


    void OnGUI()
    {
        string ipaddress = LocalIPAddress();
        GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipaddress);
        GUI.Label(new Rect(20, Screen.height - 35, 100, 20), "Status:" + NetworkServer.active);
        GUI.Label(new Rect(20, Screen.height - 20, 100, 20), "Connected:" + NetworkServer.connections.Count);
    }

    // Use this for initialization
    void Start () {

        ipaddress = LocalIPAddress();

        for (int i = 0; i < 4; i++)
        {
            HRZTAxis[i] = new CrossPlatformInputManager.VirtualAxis(hrztaxis_n + i); CrossPlatformInputManager.RegisterVirtualAxis(HRZTAxis[i]);
            VTCAxis[i] = new CrossPlatformInputManager.VirtualAxis(vtcaxis_n + i); CrossPlatformInputManager.RegisterVirtualAxis(VTCAxis[i]);
        }

        NetworkServer.Listen(25000);
        NetworkServer.RegisterHandler(578, ServerReceiveMessageJoystick); //JST - Joystick
        NetworkServer.RegisterHandler(286, ServerReceiveMessageButton); //BTN - Button
        NetworkServer.RegisterHandler(796, ServerReceiveMessagePlayerCharecter); //PYN - Player Name
        //NetworkServer.RegisterHandler(737, ServerReceiveMessageRespond); //REP - Respond
        //NetworkServer.RegisterHandler(834, ServerReceiveUpdateHealth); //UDH - Update Health 
        //NetworkServer.RegisterHandler(773, ServerReceiveResurrection); //SRE - Server REsurrection
    }



    public void ServerReceiveMessageJoystick(NetworkMessage massage)
    {
        StringMessage msg = new StringMessage();
        msg.value = massage.ReadMessage<StringMessage>().value;

        string[] deltas = msg.value.Split('|');
       HRZTAxis[int.Parse(deltas[2])].Update(float.Parse(deltas[0]));
       VTCAxis[int.Parse(deltas[2])].Update(float.Parse(deltas[1]));
    }

    public void ServerReceiveMessageButton(NetworkMessage massage)
    {
        StringMessage msg = new StringMessage();
        msg.value = massage.ReadMessage<StringMessage>().value;
        string[] deltas = msg.value.Split('|');
        playersComand.sendComand(int.Parse(deltas[1]), deltas[0]);

    }

    public void ServerReceiveMessagePlayerCharecter(NetworkMessage massage)
    {
        StringMessage msg = new StringMessage();
        msg.value = massage.ReadMessage<StringMessage>().value;

        Debug.Log(msg.value);

        for (int i = 0; i < 4; i++)
        {
            if (playersComand.players[i] == null)
            {
                playersComand.setPlayer(i, msg.value);
                SendToClientIDClient(i);
                playersCount++;
                break;
            }
        }
    }

    //public void ServerReceiveMessageRespond(NetworkMessage massage)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = massage.ReadMessage<StringMessage>().value;
    //    string[] deltas = msg.value.Split('|');
    //    playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Controller>().restext = deltas[0];
    //    if (float.Parse(deltas[0]) >= 0) playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Controller>().recres = true;
    //    else playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Controller>().recres = false;
    //}

    //public void ServerReceiveResurrection(NetworkMessage massage)
    //{
    //    Debug.Log("SUS");

    //    StringMessage msg = new StringMessage();
    //    msg.value = massage.ReadMessage<StringMessage>().value;
    //    string[] deltas = msg.value.Split('|');

    //    //50% HEALTH GAIN + SET REVIVE
    //    playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Healthpoint>().hp = playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Healthpoint>().maxhp / 2;
    //    playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Healthpoint>().isDeath = false;
    //    playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Controller>().anim.Play("Idle");

    //    //GAIN STATS
    //    Stats.beReviveGain[int.Parse(deltas[1])]++;
    //    Stats.reviveGain[int.Parse(deltas[0])]++;

    //    //DISABLE DEATH
    //    msg.value = deltas[1];
    //    NetworkServer.SendToAll(733, msg);
    //    Debug.Log(msg);

    //}

    //public void ServerReceiveUpdateHealth(NetworkMessage massage)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = massage.ReadMessage<StringMessage>().value;
    //    string[] deltas = msg.value.Split('|');

    //    playersComand.GetComponent<Checkpoint>().player[int.Parse(deltas[1])].GetComponent<Healthpoint>().hp = int.Parse(deltas[0]);
    //}

    public void SendToClientIDClient(int id)
    {
        StringMessage msg = new StringMessage();
        Debug.Log("sendID: " + id);
        msg.value = id.ToString();
        NetworkServer.SendToAll(432, msg);
    }

    //public void SendToClientDeath(int id)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = id.ToString();
    //    NetworkServer.SendToAll(338, msg);
    //}

    //public void SendToClientStatus(int id, string status, string etc)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = id + "|" + status + "|" + etc;
    //    NetworkServer.SendToAll(787, msg);
    //    Debug.Log(msg.value);
    //}

    //public void SendToClientStartGame()
    //{
    //    StringMessage msg = new StringMessage();        

    //    for (int i = 0; i < 6; i++)
    //    {
    //        if (i == 5) sidequest[i] = Random.Range(0, 3);
    //        else if (i == 4) sidequest[i] = Random.Range(0, 4);
    //        else sidequest[i] = Random.Range(0, 7);
    //        msg.value += sidequest[i] + "|";
    //    }

    //    msg.value = msg.value.Remove(msg.value.Length - 1);
    //    Debug.Log(msg.value);
    //    NetworkServer.SendToAll(784, msg);
    //}

    //public void SendToClientEndGame(string res, int[] exp)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = CheckResultSideQuest(res);
    //    for (int i = 0; i < exp.Length; i++)
    //    {
    //        msg.value += exp[i] + "|";
    //    }
    //    msg.value = msg.value.Remove(msg.value.Length - 1);
    //    NetworkServer.SendToAll(384, msg);

    //    //ClearRegisterKey();

    //    Debug.Log(msg.value);

    //}

    //public void SendToClientResurrection(string visible, int id, int target)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = visible + "|" + id + "|" + target;
    //    NetworkServer.SendToAll(777, msg);
    //}

    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                //break;
            }
        }
        return localIP;
    }

    //public void ClearRegisterKey()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        CrossPlatformInputManager.UnRegisterVirtualAxis(hrztaxis_n + i);
    //        CrossPlatformInputManager.UnRegisterVirtualAxis(vtcaxis_n + i);
    //        HRZTAxis[i] = null;
    //        VTCAxis[i] = null;
    //    }

    //    //for (int i = 0; i < 4; i++)
    //    //{
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(nmatk_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr1_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr2_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr3_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk1_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk2_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk3_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(trail_n + i);
    //    //    NMATK[i] = null;
    //    //    FR1[i] = null;
    //    //    FR2[i] = null;
    //    //    FR3[i] = null;
    //    //    SK1[i] = null;
    //    //    SK2[i] = null;
    //    //    SK3[i] = null;
    //    //    TRAIL[i] = null;
    //    //}

    //    NetworkServer.Shutdown();
    //}

    //public string CheckResultSideQuest(string mainres)
    //{
    //    string resall = "";
    //    bool isBetrayalWinSideQuest = false;

    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (i == sidequest[4])
    //        {
    //            if (sidequest[5] == 0)
    //            {
    //                for (int j = 0; j < 4; j++)
    //                {
    //                    if (playersComand.GetComponent<Checkpoint>().player[j] != null)
    //                    {
    //                        if (playersComand.GetComponent<Checkpoint>().player[j].GetComponent<Healthpoint>().hp > 0 && j != i) break;
    //                    }

    //                    isBetrayalWinSideQuest = true;
    //                }                   
    //            }

    //            else if (sidequest[5] == 1)
    //            {
    //                if (mainres == "Lose") isBetrayalWinSideQuest = true;
    //                else return "Lose|Lose|Lose|Lose|";
    //            }

    //            else if (sidequest[5] == 2)
    //            {
    //                if (Stats.effectGain[i] >= 3) isBetrayalWinSideQuest = true;
    //            }

    //            //CHECK WITH MAIN QUEST

    //            if (sidequest[i] == 0)
    //            {
    //                if (Stats.killGain[i] >= 35 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 1)
    //            {
    //                if (Stats.relicGain[i] >= 5 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 2)
    //            {
    //                if (Stats.furyGain[i] >= 25 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 3)
    //            {
    //                if (Stats.spellGain[i] >= 25 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 4)
    //            {
    //                if (Stats.trailGain[i] == 0 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 5)
    //            {
    //                if (Stats.reviveGain[i] >= 2 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 6)
    //            {
    //                if (Stats.beReviveGain[i] >= 1 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }
    //        }

    //        else
    //        {
    //            if (mainres == "Lose") resall += "Lose|";

    //            else
    //            {
    //                if (sidequest[i] == 0)
    //                {
    //                    if (Stats.killGain[i] >= 35) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 1)
    //                {
    //                    if (Stats.relicGain[i] >= 5) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 2)
    //                {
    //                    if (Stats.furyGain[i] >= 25) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 3)
    //                {
    //                    if (Stats.spellGain[i] >= 25) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 4)
    //                {
    //                    if (Stats.trailGain[i] == 0) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 5)
    //                {
    //                    if (Stats.reviveGain[i] >= 2) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 6)
    //                {
    //                    if (Stats.beReviveGain[i] >= 1) resall += "Win|";
    //                    else resall += "Lose|";
    //                }
    //            }
    //        }
    //    }

    //    return resall;
    //}

    //public string SubBetrayalWinner()
    //{
    //    string resall = "";

    //    for (int k = 0; k < 4; k++)
    //    {
    //        if (k == sidequest[4]) resall += "Win|";
    //        else resall += "Lose|";
    //    }
    //    return resall;
    //}    //public void SendToClientEndGame(string res, int[] exp)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = CheckResultSideQuest(res);
    //    for (int i = 0; i < exp.Length; i++)
    //    {
    //        msg.value += exp[i] + "|";
    //    }
    //    msg.value = msg.value.Remove(msg.value.Length - 1);
    //    NetworkServer.SendToAll(384, msg);

    //    //ClearRegisterKey();

    //    Debug.Log(msg.value);

    //}

    //public void SendToClientResurrection(string visible, int id, int target)
    //{
    //    StringMessage msg = new StringMessage();
    //    msg.value = visible + "|" + id + "|" + target;
    //    NetworkServer.SendToAll(777, msg);
    //}

    //public string LocalIPAddress()
    //{
    //    IPHostEntry host;
    //    string localIP = "";
    //    host = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (IPAddress ip in host.AddressList)
    //    {
    //        if (ip.AddressFamily == AddressFamily.InterNetwork)
    //        {
    //            localIP = ip.ToString();
    //            break;
    //        }
    //    }
    //    return localIP;
    //}

    //public void ClearRegisterKey()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        CrossPlatformInputManager.UnRegisterVirtualAxis(hrztaxis_n + i);
    //        CrossPlatformInputManager.UnRegisterVirtualAxis(vtcaxis_n + i);
    //        HRZTAxis[i] = null;
    //        VTCAxis[i] = null;
    //    }

    //    //for (int i = 0; i < 4; i++)
    //    //{
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(nmatk_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr1_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr2_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(fr3_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk1_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk2_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(sk3_n + i);
    //    //    CrossPlatformInputManager.UnRegisterVirtualButton(trail_n + i);
    //    //    NMATK[i] = null;
    //    //    FR1[i] = null;
    //    //    FR2[i] = null;
    //    //    FR3[i] = null;
    //    //    SK1[i] = null;
    //    //    SK2[i] = null;
    //    //    SK3[i] = null;
    //    //    TRAIL[i] = null;
    //    //}

    //    NetworkServer.Shutdown();
    //}

    //public string CheckResultSideQuest(string mainres)
    //{
    //    string resall = "";
    //    bool isBetrayalWinSideQuest = false;

    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (i == sidequest[4])
    //        {
    //            if (sidequest[5] == 0)
    //            {
    //                for (int j = 0; j < 4; j++)
    //                {
    //                    if (playersComand.GetComponent<Checkpoint>().player[j] != null)
    //                    {
    //                        if (playersComand.GetComponent<Checkpoint>().player[j].GetComponent<Healthpoint>().hp > 0 && j != i) break;
    //                    }

    //                    isBetrayalWinSideQuest = true;
    //                }                   
    //            }

    //            else if (sidequest[5] == 1)
    //            {
    //                if (mainres == "Lose") isBetrayalWinSideQuest = true;
    //                else return "Lose|Lose|Lose|Lose|";
    //            }

    //            else if (sidequest[5] == 2)
    //            {
    //                if (Stats.effectGain[i] >= 3) isBetrayalWinSideQuest = true;
    //            }

    //            //CHECK WITH MAIN QUEST

    //            if (sidequest[i] == 0)
    //            {
    //                if (Stats.killGain[i] >= 35 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 1)
    //            {
    //                if (Stats.relicGain[i] >= 5 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 2)
    //            {
    //                if (Stats.furyGain[i] >= 25 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 3)
    //            {
    //                if (Stats.spellGain[i] >= 25 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 4)
    //            {
    //                if (Stats.trailGain[i] == 0 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 5)
    //            {
    //                if (Stats.reviveGain[i] >= 2 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }

    //            else if (sidequest[i] == 6)
    //            {
    //                if (Stats.beReviveGain[i] >= 1 && isBetrayalWinSideQuest) return SubBetrayalWinner();
    //                else resall += "Lose|";
    //            }
    //        }

    //        else
    //        {
    //            if (mainres == "Lose") resall += "Lose|";

    //            else
    //            {
    //                if (sidequest[i] == 0)
    //                {
    //                    if (Stats.killGain[i] >= 35) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 1)
    //                {
    //                    if (Stats.relicGain[i] >= 5) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 2)
    //                {
    //                    if (Stats.furyGain[i] >= 25) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 3)
    //                {
    //                    if (Stats.spellGain[i] >= 25) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 4)
    //                {
    //                    if (Stats.trailGain[i] == 0) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 5)
    //                {
    //                    if (Stats.reviveGain[i] >= 2) resall += "Win|";
    //                    else resall += "Lose|";
    //                }

    //                else if (sidequest[i] == 6)
    //                {
    //                    if (Stats.beReviveGain[i] >= 1) resall += "Win|";
    //                    else resall += "Lose|";
    //                }
    //            }
    //        }
    //    }

    //    return resall;
    //}

    //public string SubBetrayalWinner()
    //{
    //    string resall = "";

    //    for (int k = 0; k < 4; k++)
    //    {
    //        if (k == sidequest[4]) resall += "Win|";
    //        else resall += "Lose|";
    //    }
    //    return resall;
    //}
}
