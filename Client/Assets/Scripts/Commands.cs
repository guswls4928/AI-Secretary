using System.Collections.Generic;

public class Command
{
    public string CommandText;
    public List<Command> SubCommands;

    public Command(string commandText)
        // 상위 명령어
    {
        CommandText = commandText;
        SubCommands = new List<Command>();
    }

    public void AddSubCommand(Command subCommand)
    // 하위 명령어
    {
        SubCommands.Add(subCommand);
    }
}
