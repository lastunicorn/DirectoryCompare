using System;
using System.Collections.Generic;

namespace DustInTheWind.ConsoleFramework.UserControls
{
    internal class UsageControl
    {
        public IList<string> CommandNames { get; set; }


        public void Display()
        {
            if (CommandNames == null)
                return;

            Console.WriteLine("Usage:");

            foreach (string commandName in CommandNames)
            {
                Console.WriteLine(commandName);
            }
        }
    }
}