using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    public static MyPlayer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
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

    public void SendCommand(string command)
    {
        C_Chat chatPacket = new C_Chat();
        chatPacket.message = command;

        _network.Send(chatPacket.Write());
    }
}
