using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoSendPacket());
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        yield return new WaitForSeconds(1.0f);

        C_EnterGame enter = new C_EnterGame();
        enter.playerId = 10;

        Debug.Log($"Send EnterGame Packet{enter.playerId}");
        _network.Send(enter.Write());
    }
}
