using ReservationSystem.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Shared.Abstraction
{
    public interface INetworkHttpClient
    {
        Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default);
        Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
        Task<ApiResult<TResponse>> PostAsync<TResponse>(string url, MultipartFormDataContent content, CancellationToken cancellationToken = default);
        Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
        Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string url, CancellationToken cancellationToken = default);
        Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken);

    }
}
