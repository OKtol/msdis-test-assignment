using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using Moq;

namespace GetHeadHunterVacancyStats.Tests;

public class GetVacancyStatsServiceTests
{
    private readonly Mock<IVacancyStatsReaderRepository> _readerMock = new();
    private readonly GetVacancyStatsService _service;

    public GetVacancyStatsServiceTests()
    {
        _service = new GetVacancyStatsService(_readerMock.Object);
    }

    [Fact]
    public async Task GetStatsAsync_HasData_ReturnsSerializedJson()
    {
        // Arrange
        var stats = new[]
        {
            VacancyStat.Create("2025-01-01", 42)
        };

        _readerMock.Setup(x => x.GetStatsAsync())
                   .ReturnsAsync(stats);

        // Act
        var result = await _service.GetStatsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("2025-01-01", result[0].Date);
        Assert.Equal(42, result[0].Vacancies);
    }
}