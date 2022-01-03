using System;
using System.ComponentModel;

namespace DustInTheWind.ConsoleFramework
{
    public interface ILongCommandModel : ICommandModel
    {
        event EventHandler<ProgressChangedEventArgs> Progress;
    }
}