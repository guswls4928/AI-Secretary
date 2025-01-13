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
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string basePath = Directory.GetParent(currentPath).Parent.Parent.Parent.FullName;
            string targetPath = Path.Combine(basePath, "embedded-python", "python312.dll");

            using (Py.GIL())
            {
                foreach (var file in System.IO.Directory.GetFiles(Path.Combine(basePath, "DLLs"), "*.pyd"))
                {
                    string modoleName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(file));

                    dynamic sys = Py.Import("sys");
                    sys.path.append(Path.Combine(basePath, "DLLs"));

                    dynamic os = Py.Import("os");
                    os.environ.__setitem__("DLLS_PATH", Path.Combine(basePath, "DLLs"));

                    dynamic module = Py.Import(modoleName);
                    _dllList.Add((string)module.GetName(), module.Create(10));
                }
            }
        }

        public async Task<string> Execute(C_Chat packet)
        {
            using (Py.GIL())
            {
                return _dllList[packet.module].Execute(packet.command, packet.query);
            }
        }
    }
}
