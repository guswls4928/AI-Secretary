using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_BroadcastChatHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastChat pkt = packet as S_BroadcastChat;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.ChatSync(pkt);
    }

    public static void S_ResponseHandler(PacketSession session, IPacket packet)
    {
        S_Response pkt = packet as S_Response;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Response(pkt);
    }
}
