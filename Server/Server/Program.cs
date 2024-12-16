using System.Net;
using Python.Runtime;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		public static GameRoom Room = new GameRoom();

		static void FlushRoom()
		{
			Room.Push(() => Room.Flush());
			JobTimer.Instance.Push(FlushRoom, 250);
		}

		static void Main(string[] args)
		{
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string basePath = Directory.GetParent(currentPath).Parent.Parent.Parent.FullName;
            string targetPath = Path.Combine(basePath, "embedded-python", "python312.dll");

            Runtime.PythonDLL = targetPath;
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

			DLLManager.Instance.SetModule();

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

            //FlushRoom();
            JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				JobTimer.Instance.Flush();
			}
		}
	}
}
