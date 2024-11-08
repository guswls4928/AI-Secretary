using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

class PacketHandler
{
	public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
	{
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;

		GameRoom room = clientSession.Room;
		room.Push(() => room.Leave(clientSession));
	}

    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Chat(clientSession, chatPacket));
    }

    public static void C_EnterGameHandler(PacketSession session, IPacket packet)
    {
        C_EnterGame enterPacket = packet as C_EnterGame;

        ClientSession clientSession = session as ClientSession;
        Program.Room.Push(() => Program.Room.Enter(clientSession, enterPacket));
    }
}
