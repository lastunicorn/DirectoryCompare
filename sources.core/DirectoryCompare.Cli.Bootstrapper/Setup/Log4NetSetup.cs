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
            string assemblyFilePath = Assembly.GetEntryAssembly().Location;
            string applicationDirectoryPath = Path.GetDirectoryName(assemblyFilePath);
            string log4NetFilePath = Path.Combine(applicationDirectoryPath, "Log4Net.config");
            XmlConfigurator.Configure(logRepo, new FileInfo(log4NetFilePath));
        }
    }
}