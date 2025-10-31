using GetHeadHunterVacancyStats;
using GetHeadHunterVacancyStats.Models;
using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using Moq;
using Xunit;

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
    public async Task GetStatsAsync_HasData_ReturnsResponseWithStats()
    {
        // Arrange
        var stats = new[]
        {
            new VacancyStat
            {
                Date = "2025-01-01",
                Vacancies = new Jobs { CSharpVacanciesCount = 42 }
            }
        };

        _readerMock.Setup(x => x.GetStatsAsync())
                   .ReturnsAsync(stats);

        // Act
        var result = await _service.GetStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Data load successfully", result.Message);
        Assert.Single(result.Vacancies);
        Assert.Equal("2025-01-01", result.Vacancies[0].Date);
        Assert.Equal(42, result.Vacancies[0].Vacancies.CSharpVacanciesCount);
    }
}
