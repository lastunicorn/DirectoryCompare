using System.Threading.Tasks;

namespace DustInTheWind.DirectoryCompare.Infrastructure
{
    public interface IRequestBus
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request);
        Task SendAsync<TRequest>(TRequest request);
    }
}