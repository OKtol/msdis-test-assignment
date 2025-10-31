using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using HeadHunterVacancyStats.Infrastructure.Services;
using Moq;
using System.Text.Json;

namespace HeadHunterVacancyStats.Infrastructure.Tests;

public class VacancyStatsReaderRepositoryTests
{
    private readonly Mock<IBaseS3Repository> _s3RepoMock = new();
    private readonly VacancyStatsReaderRepository _repository;

    public VacancyStatsReaderRepositoryTests()
    {
        _repository = new VacancyStatsReaderRepository(_s3RepoMock.Object);
    }

    [Fact]
    public async Task GetStatsAsync_EmptyBucket_ReturnsEmptyArray()
    {
        // Arrange
        _s3RepoMock.Setup(x => x.GetObjectStringAsync())
                   .ReturnsAsync((string?)null);

        // Act
        var result = await _repository.GetStatsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStatsAsync_ValidJson_ReturnsStats()
    {
        // Arrange
        var stats = new[]
        {
            new VacancyStat { Date = "2025-01-01", Vacancies = new Jobs { CSharpVacanciesCount = 42 } }
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        var json = JsonSerializer.Serialize(stats, options);

        _s3RepoMock.Setup(x => x.GetObjectStringAsync())
                   .ReturnsAsync(json);

        // Act
        var result = await _repository.GetStatsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("2025-01-01", result[0].Date);
        Assert.Equal(42, result[0].Vacancies.CSharpVacanciesCount);
    }

    [Fact]
    public async Task GetStatsAsync_InvalidJson_ThrowsException()
    {
        // Arrange
        var invalidJson = "{invalid:json}";
        _s3RepoMock.Setup(x => x.GetObjectStringAsync())
                   .ReturnsAsync(invalidJson);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.GetStatsAsync());
    }

    [Fact]
    public async Task GetStatsAsync_S3Error_ThrowsException()
    {
        // Arrange
        _s3RepoMock.Setup(x => x.GetObjectStringAsync())
                   .ThrowsAsync(new Amazon.S3.AmazonS3Exception("Test error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.GetStatsAsync());
        Assert.Contains("Failed to access stats data", ex.Message);
    }
}
