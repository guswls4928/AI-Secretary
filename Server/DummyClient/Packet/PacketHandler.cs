using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S_BroadcastChatHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastChat pkt = packet as S_BroadcastChat;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S_ResponseHandler(PacketSession session, IPacket packet)
    {
        S_Response pkt = packet as S_Response;
        ServerSession serverSession = session as ServerSession;
    }
}
