using HeadHunterVacancyStats.Domain.Models;

namespace HeadHunterVacancyStats.Domain.Tests;

public class VacancyStatTests
{
    [Fact]
    public void Create_ValidInput_ReturnsVacancyStat()
    {
        // Arrange
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var jobs = new Jobs { CSharpVacanciesCount = 42 };

        // Act
        var stat = new VacancyStat { Date = date, Vacancies = jobs };

        // Assert
        Assert.Equal(date, stat.Date);
        Assert.Same(jobs, stat.Vacancies);
        Assert.Equal(42, stat.Vacancies.CSharpVacanciesCount);
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("invalid-date", typeof(ArgumentException))]
    public void Create_InvalidDate_ThrowsArgumentException(string? date, Type exceptionType)
    {
        // Act & Assert
        var ex = Assert.Throws(exceptionType, () =>
        {
            // use null-forgiving where needed; for empty/invalid date pass as-is
            var _ = new VacancyStat { Date = date!, Vacancies = new Jobs { CSharpVacanciesCount = 1 } };
        });

        Assert.IsType(exceptionType, ex);
    }

    [Fact]
    public void CreatingWithoutVacancies_IsNotAllowedByCompilerButRuntimeCheckNotNeeded()
    {
        // This test exists to document that Vacancies is required (init-only).
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var stat = new VacancyStat { Date = date, Vacancies = new Jobs { CSharpVacanciesCount = 0 } };
        Assert.NotNull(stat.Vacancies);
    }
}
