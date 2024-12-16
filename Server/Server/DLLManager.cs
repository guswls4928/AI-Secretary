using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DLLManager
    {
        static DLLManager _instance = new DLLManager();
        public static DLLManager Instance { get { return _instance; } }

        Dictionary<string, dynamic> _dllList = new Dictionary<string, dynamic>();

        public void SetModule()
        {
            using (Py.GIL())
            {
                foreach (var file in System.IO.Directory.GetFiles("C:\\Users\\Hyeon\\project\\AI-Secretary\\Server\\Server\\DLLs\\", "*.pyd"))
                {
                    string modoleName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(file));

                    dynamic sys = Py.Import("sys");
                    sys.path.append("C:\\Users\\Hyeon\\project\\AI-Secretary\\Server\\Server\\DLLs\\");

                    dynamic os = Py.Import("os");
                    os.environ.__setitem__("DLLS_PATH", "C:\\Users\\Hyeon\\project\\AI-Secretary\\Server\\Server\\DLLs");

                    dynamic module = Py.Import(modoleName);
                    _dllList.Add((string)module.GetName(), module.Create(10));
                }
            }
        }

        public string Execute(C_Chat packet)
        {
            using (Py.GIL())
            {
                return _dllList[packet.module].Execute(packet.command, packet.query);
            }
        }
    }
}
