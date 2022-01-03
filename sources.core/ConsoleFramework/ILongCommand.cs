using System;
using System.ComponentModel;

namespace DustInTheWind.ConsoleFramework
{
    public interface ILongCommand : ICommand
    {
        event EventHandler<ProgressChangedEventArgs> Progress;
    }
}