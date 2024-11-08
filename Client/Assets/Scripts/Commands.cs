using System.Collections.Generic;

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

