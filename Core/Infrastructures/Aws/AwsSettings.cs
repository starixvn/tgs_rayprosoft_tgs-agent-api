namespace TGS.Core.Infrastructures.Aws
{
    public record AwsSettings
    {
        public string AccessKeyId { get; init; }
        public string SecretAccessKey { get; init; }
        public string Region { get; init; }
        public string ServiceUrl { get; init; }
        public bool UseHttp { get; init; }
        public S3 S3 { get; init; }
        public CloudFront CloudFront { get; init; }
        public CloudWatch CloudWatch { get; init; }
    }

    public record S3
    {
        public string ServiceUrl { get; init; }
        public string Bucket { get; init; }
    }

    public record CloudFront
    {
        public string Url { get; init; }
    }

    public record CloudWatch
    {
        public string ServiceUrl { get; init; }
        public string GroupName { get; init; }
    }
}