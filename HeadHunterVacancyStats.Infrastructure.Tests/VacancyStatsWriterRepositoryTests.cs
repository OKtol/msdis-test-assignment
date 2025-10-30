using Amazon.S3;
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
        _repository = new VacancyStatsWriterRepository(_s3RepoMock.Object);
    }

    [Fact]
    public async Task SaveDailyStatsAsync_ValidInput_SavesToS3()
    {
        // Arrange
        var date = "2025-01-01";
        var count = 42;

        string? capturedJson = null;
        _s3RepoMock.Setup(x => x.PutObjectStringAsync(It.IsAny<string>()))
                   .Callback<string>(json => capturedJson = json)
                   .Returns(Task.CompletedTask);

        // Act
        await _repository.SaveDailyStatsAsync(date, count);

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
        _s3RepoMock.Setup(x => x.PutObjectStringAsync(It.IsAny<string>()))
                   .ThrowsAsync(new AmazonS3Exception("Test error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SaveDailyStatsAsync("2025-01-01", 42));
        Assert.Contains("Failed to save stats data", ex.Message);
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public async Task SaveDailyStatsAsync_InvalidDate_ThrowsException(string? date, Type exceptionType)
    {
        // Act & Assert
        await Assert.ThrowsAsync(exceptionType,
            () => _repository.SaveDailyStatsAsync(date!, 42));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task SaveDailyStatsAsync_NegativeCount_ThrowsException(int count)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _repository.SaveDailyStatsAsync("2025-01-01", count));
    }
}