using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Cli.Commands;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        protected override CommandCollection CreateCommands()
        {
            ReadDiskCommand readDiskCommand = new ReadDiskCommand();
            ReadFileCommand readFileCommand = new ReadFileCommand();
            VerifyDiskCommand verifyDiskCommand = new VerifyDiskCommand();

            return new CommandCollection
            {
                { "read-disk", readDiskCommand },
                { "read", readDiskCommand },
                { "read-file", readFileCommand },
                { "view", readFileCommand },
                { "verify-disk", verifyDiskCommand },
                { "check", verifyDiskCommand },
                { "compare-disks", new CompareDisksCommand() },
                { "compare-files", new CompareFilesCommand() },
                { "find-duplicates", new FindDuplicatesCommand() },
                { "remove-duplicates", new RemoveDuplicatesCommand() }
            };
        }
    }
}