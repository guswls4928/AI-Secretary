using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    private static MyPlayer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static MyPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoSendPacket());
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    IEnumerator CoSendPacket()
    {
        yield return new WaitForSeconds(1.0f);

        C_EnterGame enter = new C_EnterGame();
        enter.playerId = 10;

        Debug.Log($"Send EnterGame Packet{enter.playerId}");
        _network.Send(enter.Write());
    }

    public void SendCommand(string command, string query = "")
    {
        C_Chat chatPacket = new C_Chat();
        chatPacket.command = command;
        chatPacket.query = query;

        _network.Send(chatPacket.Write());
    }
}
