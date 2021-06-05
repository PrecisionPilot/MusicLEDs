using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;

public class NetworkMGR : NetworkManager
{
    [SerializeField]
    InputField ipAddress;
    [SerializeField]
    InputField port;
    [SerializeField]
    GameObject error;
    [SerializeField]
    Text networkHostAddress;
    [SerializeField]
    Transform Canvas;

    private void Start()
    {
        string localIPAddress = LocalIPAddress();
        networkHostAddress.text = "IP Address: " + localIPAddress;

        ipAddress.text = localIPAddress;

        GameObject.Find("Host Button").GetComponent<Button>().onClick.AddListener(() => StartUpHost());
        GameObject.Find("Join Button").GetComponent<Button>().onClick.AddListener(() => StartUpClient());
        GameObject.Find("Disconnect Button").GetComponent<Button>().onClick.AddListener(() => StopHost());
    }
    public void StartUpHost()
    {
        try
        {
            singleton.networkAddress = ipAddress.text;
            singleton.networkPort = int.Parse(port.text);
            singleton.StartHost();
            AudioVisualizer.isMultiplayer = true;
        }
        catch
        {
            Instantiate(error, Canvas);
        }
    }
    public void StartUpClient()
    {
        try
        {
            singleton.networkAddress = ipAddress.text;
            singleton.networkPort = int.Parse(port.text);
            singleton.StartClient();
            AudioVisualizer.isMultiplayer = false;
        }
        catch
        {
            Instantiate(error, Canvas);
        }
    }

    string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
