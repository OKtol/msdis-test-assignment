namespace HeadHunterVacancyStats.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchTextAttribute : Attribute
{
    public string Text { get; }

    public SearchTextAttribute(string searchText)
    {
        Text = searchText;
    }
}
