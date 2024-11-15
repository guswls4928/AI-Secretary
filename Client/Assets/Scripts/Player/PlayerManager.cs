using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerManager
{
    MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        Debug.Log(packet.message);
    }

    public void ChatSync(S_BroadcastChat packet)
    {
        Debug.Log(packet.message);
    }

    public void Response(S_Response packet)
    {
        Debug.Log(packet.message);
    }
}
