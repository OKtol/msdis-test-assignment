using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using HeadHunterVacancyStats.Infrastructure.Services;
using Moq;

namespace HeadHunterVacancyStats.Infrastructure.Tests;

public class VacancyStatsWriterRepositoryTests
{
    private readonly Mock<IBaseS3Repository> _s3RepoMock = new();
    private readonly VacancyStatsWriterRepository _repository;

    public VacancyStatsWriterRepositoryTests()
    {
        // By default mock.GetObjectStringAsync returns null so initialization leaves dictionary empty
        _repository = new VacancyStatsWriterRepository(_s3RepoMock.Object);
    }

    [Fact]
    public async Task SaveDailyStatsAsync_ValidInput_SavesToS3()
    {
        // Arrange
        var date = "2025-01-01";
        var stat = new VacancyStat { Date = date, Vacancies = new Jobs { CSharpVacanciesCount = 42 } };

        string? capturedJson = null;
        _s3RepoMock.Setup(x => x.PutObjectStringAsync(It.IsAny<string>()))
                   .Callback<string>(json => capturedJson = json)
                   .Returns(Task.CompletedTask);

        // Act
        await _repository.SaveDailyStatsAsync(stat);

        // Assert
        _s3RepoMock.Verify(x => x.PutObjectStringAsync(It.IsAny<string>()), Times.Once);
        Assert.NotNull(capturedJson);
        Assert.Contains(date, capturedJson);
        Assert.Contains("42", capturedJson);
    }

    [Fact]
    public async Task SaveDailyStatsAsync_S3Error_ThrowsException()
    {
        // Arrange
        var stat = new VacancyStat { Date = "2025-01-01", Vacancies = new Jobs { CSharpVacanciesCount = 42 } };

        _s3RepoMock.Setup(x => x.PutObjectStringAsync(It.IsAny<string>()))
                   .ThrowsAsync(new Amazon.S3.AmazonS3Exception("Test error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SaveDailyStatsAsync(stat));
        Assert.Contains("Failed to save stats data", ex.Message);
    }
}
