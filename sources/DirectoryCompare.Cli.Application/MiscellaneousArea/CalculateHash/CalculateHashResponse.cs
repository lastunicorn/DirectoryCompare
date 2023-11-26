using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CalculateHash;

public class CalculateHashResponse
{
    public FileHash Hash { get; set; }
}