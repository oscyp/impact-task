using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace Tenders.Guru.Facade.Api.Tests.Testing;

public static class MockHttpMessageHandler
{
    public static HttpClient CreateWithResponse<T>(T responseObject, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(responseObject);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = new HttpResponseMessage(statusCode)
        {
            Content = httpContent
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://tenders.guru/api/pl/")
        };
    }

    public static HttpClient CreateWithNotFound()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://tenders.guru/api/pl/")
        };
    }

    public static HttpClient CreateWithMultipleResponses(params HttpResponseMessage[] responses)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var setup = handlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        foreach (var response in responses)
        {
            setup = setup.ReturnsAsync(response);
        }

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://tenders.guru/api/pl/")
        };
    }
}
