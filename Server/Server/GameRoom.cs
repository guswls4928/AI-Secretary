using Python.Runtime;
using ServerCore;
using System;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        string? moduleName = null;
        dynamic func;

        public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		public void Flush()
		{
			foreach (ClientSession s in _sessions)
				s.Send(_pendingList);

			//Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();
		}

		public void Broadcast(ArraySegment<byte> segment)
		{
			_pendingList.Add(segment);	
		}

		public void Enter(ClientSession session, C_EnterGame packet)
		{
			Console.WriteLine($"Enter Player: {packet.playerId}");

            _sessions.Add(session);
			session.Room = this;
			session.PlayerId = packet.playerId;

            S_BroadcastEnterGame history = new S_BroadcastEnterGame();
			history.message = "";
			if(session.message != null)
                history.message = session.message;

            session.Send(history.Write());
        }

		public void Leave(ClientSession session)
		{
			_sessions.Remove(session);
        }

        public void SetModule(string name)
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append("C:\\Users\\Hyeon\\project\\AI-Secretary\\Server\\Server\\DLLs\\");

                dynamic os = Py.Import("os");
                os.environ.__setitem__("DLLS_PATH", "C:\\Users\\Hyeon\\project\\AI-Secretary\\Server\\Server\\DLLs");

                dynamic module = Py.Import("music");
                func = module.Create(10);
            }
        }

        public void Chat(ClientSession session, C_Chat packet)
		{
			session.message = packet.command;
			Console.WriteLine($"{session.PlayerId}'s Chat : {packet.command}");

			if (moduleName == null)
			{
				moduleName = packet.command;
                SetModule(moduleName);
                return;
            }
			else if (packet.command == "상위")
			{
				moduleName = null;
				return;
            }

            dynamic res;

			using (Py.GIL())
			{
                res = func.Execute(packet.command, "귀여움");
            }

            S_Response response = new S_Response();
			response.response = (string)res;
			Broadcast(response.Write());
		}

    }
}
