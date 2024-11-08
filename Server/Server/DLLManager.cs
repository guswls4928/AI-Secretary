using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace Server
{
    public class Command
    {
        public string CommandText;
        public List<Command> SubCommands;
        public Command ParentCommand;

        public Command(string commandText, Command parent = null)
        {
            CommandText = commandText;
            SubCommands = new List<Command>();
            ParentCommand = parent;
        }

        public void AddSubCommand(Command subCommand)
        {
            subCommand.ParentCommand = this;
            SubCommands.Add(subCommand);
        }
    }


    internal class DLLManager
    {
        public static DLLManager instance;
        private static bool isInitialized = false;
        public static Command rootCommand;

        public static DLLManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DLLManager();
                }
                return instance;
            }
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            Runtime.PythonDLL = @"C:\Users\Hyeon\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
            isInitialized = true;
        }

        public void SetCommands()
        {
            Initialize();

            rootCommand = new Command("Home");
            DirectoryInfo di = new DirectoryInfo("../../../DLLs");

            foreach (var fi in di.GetFiles())
            {
                if (fi.Extension != ".pyd")
                {
                    continue;
                }
                string pythonPath = @"C:\Users\Hyeon\project\AI-Secretary\Server\Server\DLLs";
                PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Path.GetFullPath(pythonPath);
                using (Py.GIL())
                {
                    dynamic os = Py.Import("os");
                    dynamic sys = Py.Import("sys");
                    sys.path.append(pythonPath);

                    try
                    {
                        string moduleName = Path.GetFileNameWithoutExtension(fi.Name);
                        dynamic module = Py.Import(moduleName);

                        string name = module.GetName();
                        Command newCommand = new Command(name);

                        foreach (dynamic command in module.GetCommands())
                        {
                            newCommand.AddSubCommand(new Command(command.ToString()));
                        }
                        rootCommand.AddSubCommand(newCommand);
                    }
                    catch (PythonException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
