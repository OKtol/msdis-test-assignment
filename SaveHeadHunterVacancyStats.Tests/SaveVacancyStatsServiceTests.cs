using HeadHunterVacancyStats.Domain.Models;
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
    public async Task SaveTodayStatsAsync_ValidData_SavesAndReturnsResponse()
    {
        // Arrange
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var encodedSearch = "C%23%20Developer"; // as defined on Jobs property

        _clientMock.Setup(x => x.GetVacanciesFoundAsync(encodedSearch))
                   .ReturnsAsync(917);

        VacancyStat? captured = null;
        _writerMock.Setup(x => x.SaveDailyStatsAsync(It.IsAny<VacancyStat>()))
                   .Callback<VacancyStat>(v => captured = v)
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SaveTodayStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Data saved successfully", result.Message);
        Assert.NotNull(captured);
        Assert.Equal(today, captured!.Date);
        Assert.Equal(917, captured.Vacancies.CSharpVacanciesCount);

        _writerMock.Verify(x => x.SaveDailyStatsAsync(It.IsAny<VacancyStat>()), Times.Once);
    }

    [Fact]
    public async Task SaveTodayStatsAsync_RepositoryError_ThrowsException()
    {
        // Arrange
        var encodedSearch = "C%23%20Developer";
        _clientMock.Setup(x => x.GetVacanciesFoundAsync(encodedSearch))
                   .ReturnsAsync(100);

        _writerMock.Setup(x => x.SaveDailyStatsAsync(It.IsAny<VacancyStat>()))
                   .ThrowsAsync(new Exception("Test error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.SaveTodayStatsAsync());
        Assert.Contains("Failed to save vacancy stats", ex.Message);
    }
}
