using Amazon.S3;
using Amazon.S3.Model;
using HeadHunterVacancyStats.Infrastructure.Interfaces;

namespace HeadHunterVacancyStats.Infrastructure.Services;

public class BaseS3Repository : IBaseS3Repository
{
    private const string _statsFileName = "vacancies_stats.json";
    private readonly string _bucket;
    private readonly IAmazonS3 _client;

    public BaseS3Repository(string bucket, IAmazonS3 client)
    {
        ArgumentNullException.ThrowIfNull(bucket);

        _bucket = bucket;
        _client = client;
    }

    public async Task<string?> GetObjectStringAsync()
    {
        try
        {
            var resp = await _client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _bucket,
                Key = _statsFileName
            });
            using var sr = new StreamReader(resp.ResponseStream, System.Text.Encoding.UTF8);
            return await sr.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task PutObjectStringAsync(string content)
    {
        var put = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = _statsFileName,
            ContentBody = content,
            ContentType = "application/json; charset=utf-8"
        };
        await _client.PutObjectAsync(put);
    }
}