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
            if (PythonEngine.IsInitialized)
            {
                return;
            }
            Runtime.PythonDLL = @"C:\Users\Hyeon\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
        }

        
    }
}
