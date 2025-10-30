using Moq;
using Moq.Protected;
using System.Net;

namespace SaveHeadHunterVacancyStats.Tests;

public class HeadHunterClientTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HeadHunterClient _client;
    private const string TestUrl = "http://test-url";

    public HeadHunterClientTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_handlerMock.Object);
        _client = new HeadHunterClient(httpClient, TestUrl);
    }

    [Fact]
    public void Constructor_NullBaseUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HeadHunterClient(httpClient, null!));
    }

    [Fact]
    public async Task GetCSharpVacanciesFoundAsync_ValidResponse_ReturnsCount()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""found"": 42}")
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _client.GetCSharpVacanciesFoundAsync();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task GetCSharpVacanciesFoundAsync_InvalidResponse_ThrowsException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _client.GetCSharpVacanciesFoundAsync());
        Assert.Contains("HH API request failed", ex.Message);
    }

    [Fact]
    public async Task GetCSharpVacanciesFoundAsync_InvalidJsonResponse_ThrowsException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _client.GetCSharpVacanciesFoundAsync());
        Assert.Contains("Response does not contain 'found'", ex.Message);
    }

    [Fact]
    public async Task GetCSharpVacanciesFoundAsync_NetworkError_ThrowsException()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _client.GetCSharpVacanciesFoundAsync());
        Assert.Contains("Failed to get vacancy count from HH API", ex.Message);
    }
}
