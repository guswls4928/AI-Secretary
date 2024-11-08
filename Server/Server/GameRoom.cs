using ServerCore;
using Newtonsoft.Json;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

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
			history.message = "test";

            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            history.commands = JsonConvert.SerializeObject(DLLManager.rootCommand, settings);

            session.Send(history.Write());
        }

		public void Leave(ClientSession session)
		{
			_sessions.Remove(session);
        }

        public void Chat(ClientSession session, C_Chat packet)
		{
			session.message = packet.message;

            S_BroadcastChat chat = new S_BroadcastChat();
            chat.message = session.message;
            Broadcast(chat.Write());
        }

    }
}
