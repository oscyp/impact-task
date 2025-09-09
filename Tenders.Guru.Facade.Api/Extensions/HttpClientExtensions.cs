using Tenders.Guru.Facade.Api.Exceptions;

namespace Tenders.Guru.Facade.Api.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetFromJsonAsyncSafe<T>(this HttpClient httpClient, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<T>(requestUri, cancellationToken);
            
            if (response is null)
            {
                throw new TendersApiException($"Received null response from {requestUri}");
            }
            
            return response;
        }
        catch (HttpRequestException ex)
        {
            throw new TendersApiException($"Error fetching data from {requestUri}: {ex.Message}", ex);
        }
    }
}
