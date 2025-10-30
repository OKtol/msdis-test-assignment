using HeadHunterVacancyStats.Domain.Models;

namespace HeadHunterVacancyStats.Domain.Tests;

public class VacancyStatTests
{
    [Fact]
    public void Create_ValidInput_ReturnsVacancyStat()
    {
        // Arrange
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var vacancies = 42;

        // Act
        var stat = VacancyStat.Create(date, vacancies);

        // Assert
        Assert.Equal(date, stat.Date);
        Assert.Equal(vacancies, stat.Vacancies);
    }

    [Theory]
    [InlineData(null, "Date cannot be empty")]
    [InlineData("", "Date cannot be empty")]
    [InlineData("invalid-date", "Date must be in format yyyy-MM-dd")]
    public void Create_InvalidDate_ThrowsArgumentException(string? date, string expectedMessage)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            VacancyStat.Create(date!, 42));
        Assert.Contains(expectedMessage, ex.Message);
    }

    [Fact]
    public void Create_NegativeVacancies_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            VacancyStat.Create("2025-01-01", -1));
        Assert.Contains("Vacancies count cannot be negative", ex.Message);
    }
}