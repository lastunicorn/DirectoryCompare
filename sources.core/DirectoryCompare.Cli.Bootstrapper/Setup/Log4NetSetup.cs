using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal class Log4NetSetup
    {
        public static void Configure()
        {
            ILoggerRepository logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, new FileInfo("Log4Net.config"));
        }
    }
}