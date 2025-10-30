using Amazon.S3;
using Amazon.S3.Model;

namespace VacancyStatsFromHeadHunter;

public static class S3Helper
{
    private static AmazonS3Client CreateClient()
    {
        var accessKey = Environment.GetEnvironmentVariable("S3_ACCESS_KEY")
                        ?? throw new InvalidOperationException("S3_ACCESS_KEY missing");
        var secretKey = Environment.GetEnvironmentVariable("S3_SECRET_KEY")
                        ?? throw new InvalidOperationException("S3_SECRET_KEY missing");

        var config = new AmazonS3Config
        {
            ServiceURL = "https://s3.yandexcloud.net",
            ForcePathStyle = true
        };
        return new AmazonS3Client(accessKey, secretKey, config);
    }

    public static async Task<string?> GetObjectStringAsync(string bucket, string key)
    {
        using var client = CreateClient();
        try
        {
            var resp = await client.GetObjectAsync(new GetObjectRequest { BucketName = bucket, Key = key });
            using var sr = new StreamReader(resp.ResponseStream, System.Text.Encoding.UTF8);
            return await sr.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public static async Task PutObjectStringAsync(string bucket, string key, string content)
    {
        using var client = CreateClient();
        var put = new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            ContentBody = content,
            ContentType = "application/json; charset=utf-8"
        };
        await client.PutObjectAsync(put);
    }
}
