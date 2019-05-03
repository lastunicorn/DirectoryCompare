using System;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Cli
{
    public class CommandCollection
    {
        protected readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

        public void Add(string key, ICommand command)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (Commands.ContainsKey(key))
                throw new ArgumentException("There is another command with the same key.", nameof(key));

            Commands.Add(key, command);
        }

        public ICommand SelectCommand(Arguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.Command))
                throw new Exception("Please provide a command name to execute.");

            if (!Commands.ContainsKey(arguments.Command))
                throw new Exception("Invalid command.");

            ICommand command = Commands[arguments.Command];
            command.Initialize(arguments);

            return command;
        }
    }
}