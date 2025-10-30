using HeadHunterVacancyStats.Infrastructure.Interfaces;
using Moq;

namespace SaveHeadHunterVacancyStats.Tests;

public class SaveVacancyStatsServiceTests
{
    private readonly Mock<IHeadHunterClient> _clientMock = new();
    private readonly Mock<IVacancyStatsWriterRepository> _writerMock = new();
    private readonly SaveVacancyStatsService _service;

    public SaveVacancyStatsServiceTests()
    {
        _service = new SaveVacancyStatsService(_clientMock.Object, _writerMock.Object);
    }

    [Fact]
    public async Task SaveTodayStatsAsync_ValidData_SavesAndReturnsOk()
    {
        // Arrange
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        _clientMock.Setup(x => x.GetCSharpVacanciesFoundAsync())
                  .ReturnsAsync(42);
        
        _writerMock.Setup(x => x.SaveDailyStatsAsync(today, 42))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SaveTodayStatsAsync();

        // Assert
        Assert.Contains("OK:", result);
        Assert.Contains(today, result);
        Assert.Contains("42", result);
        _writerMock.Verify(x => x.SaveDailyStatsAsync(today, 42), Times.Once);
    }

    [Fact]
    public async Task SaveTodayStatsAsync_RepositoryError_ThrowsException()
    {
        // Arrange
        _clientMock.Setup(x => x.GetCSharpVacanciesFoundAsync())
                  .ReturnsAsync(42);

        _writerMock.Setup(x => x.SaveDailyStatsAsync(It.IsAny<string>(), It.IsAny<int>()))
                   .ThrowsAsync(new Exception("Test error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.SaveTodayStatsAsync());
        Assert.Contains("Failed to save vacancy stats", ex.Message);
    }
}