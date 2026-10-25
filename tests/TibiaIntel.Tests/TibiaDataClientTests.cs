using System.Net;
using TibiaIntel.External.TibiaData;

namespace TibiaIntel.Tests;

public sealed class TibiaDataClientTests
{
    [Fact]
    public async Task CharacterNotFoundPreservesV5NotFoundStatus()
    {
        using var handler = new NotFoundHandler();
        using var http = new HttpClient(handler)
        {
            BaseAddress = new Uri(new TibiaDataOptions().BaseUrl)
        };
        var client = new TibiaDataClient(http);

        var response = await client.GetCharacterAsync("Missing Character", default);

        Assert.EndsWith("/v5/character/Missing%20Character", handler.LastUrl);
        Assert.False(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class NotFoundHandler : HttpMessageHandler
    {
        public string? LastUrl { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastUrl = request.RequestUri!.ToString();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
