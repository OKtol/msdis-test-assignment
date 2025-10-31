namespace HeadHunterVacancyStats.Domain.Models.HttpResponse;

public class HttpResponseEnvelope
{
    public bool IsBase64Encoded { get; set; } = false;
    public int StatusCode { get; set; }
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public string Body { get; set; } = string.Empty;
}

