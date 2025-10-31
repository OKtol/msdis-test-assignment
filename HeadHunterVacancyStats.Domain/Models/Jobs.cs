using HeadHunterVacancyStats.Domain.Attributes;
using System.Text.Json.Serialization;

namespace HeadHunterVacancyStats.Domain.Models;

public class Jobs
{
    [SearchText("C%23%20Developer")]
    [JsonPropertyName("C# Developer")]
    public int CSharpVacanciesCount { get; set; }
}
