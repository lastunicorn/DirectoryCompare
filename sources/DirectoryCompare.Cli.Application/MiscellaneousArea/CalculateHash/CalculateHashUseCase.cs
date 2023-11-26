using System.Security.Cryptography;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CalculateHash;

internal class CalculateHashUseCase : IRequestHandler<CalculateHashRequest, CalculateHashResponse>
{
    public Task<CalculateHashResponse> Handle(CalculateHashRequest request, CancellationToken cancellationToken)
    {
        using Stream stream = File.OpenRead(request.Path);

        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(stream);

        CalculateHashResponse response = new()
        {
            Hash = hash, 
        };

        return Task.FromResult(response);
    }
}