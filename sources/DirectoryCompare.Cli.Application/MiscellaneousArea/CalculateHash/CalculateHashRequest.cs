using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CalculateHash;

public class CalculateHashRequest : IRequest<CalculateHashResponse>
{
    public string Path { get; set; }
}