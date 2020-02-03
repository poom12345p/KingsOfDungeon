using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery {

    private Dictionary<LanConnectionInfo, float> ipInfo = new Dictionary<LanConnectionInfo, float>();

    float timeout = 3f;

    const int key = 2156;
    const int port = 7878;

    IEnumerator CleanEndtries;

    private void Awake()
    {
        broadcastKey = key;
        broadcastPort = port;
        showGUI = false;
        CleanEndtries = CleanOldEndtries();
    }

    // Server side
    public void StartBoardCast(string roomName, int GamePort)
    {
        // base
        try
        {
            broadcastData = roomName + "|" + GamePort;
            Initialize();
            StartAsServer();
        }
        catch (System.Exception e)
        {
            print(e);
        }
        
    }

    // client side
    public void StartListenOnBoardCast()
    {
        // base
        try
        {
            Initialize();
            StartAsClient();

            StartCoroutine(CleanEndtries);
        }
        catch (System.Exception e)
        {
            print(e);
        }
        
    }

    // when recieve data
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        LanConnectionInfo newLan = new LanConnectionInfo(fromAddress, data);

        if (!ipInfo.ContainsKey(newLan))
        {
            ipInfo.Add(newLan, Time.time + timeout);
            UpdateLanConnection();
        }
        else
        {
            ipInfo[newLan] = Time.time + timeout;
        }
        
    }

    IEnumerator CleanOldEndtries()
    {
        while (true)
        {
            bool update = false;
            var keys = ipInfo.Keys.ToList();
            foreach (var key in keys)
            {
                if(ipInfo[key] <= Time.time)
                {
                    ipInfo.Remove(key);
                    update = true;
                }
            }

            if (update)
            {
                UpdateLanConnection();
            }

            yield return new WaitForSeconds(timeout);
        }
        
    }

    public void UpdateLanConnection()
    {
        GameManagerMobile.Instance.AddLanConnection(ipInfo.Keys.ToList());
    }

    public void EndListening()
    {
        try{
            if (running)
            {
                StopBroadcast();
                StopCoroutine(CleanEndtries);
            }
        }catch(System.Exception e){
            print(e);
        }
        
    }

    public void EndBroadCasting()
    {
        try{
            broadcastData = "";
            StopBroadcast();
        }catch(System.Exception e){
            print(e);
        }
        
    }

}

public struct LanConnectionInfo
{
    public string ip;
    public int port;
    public string roomName;

    public LanConnectionInfo(string fromAddress, string data)
    {
        ip = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
        string[] deltas = data.Split('|');
        roomName = deltas[0];
        port = 25000;
        int.TryParse(deltas[1], out port);
    }

    public override string ToString()
    {
        return string.Format("ip= {0}, port= {1}, name= {2}", ip, port, roomName);
    }
}
