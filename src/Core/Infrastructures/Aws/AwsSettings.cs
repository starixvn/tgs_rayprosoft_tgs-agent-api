namespace TGS.Core.Infrastructures.Aws
{
    public record AwsSettings
    {
        public string AccessKeyId { get; init; }
        public string SecretAccessKey { get; init; }
        public string Region { get; init; }
        public string ServiceUrl { get; init; }
        public bool UseHttp { get; init; }
        public S3 Project { get; init; }
        public S3 Secret { get; init; }
    }

    public record S3
    {
        public string ExternalServiceUrl { get; init; }
        public string ServiceUrl { get; init; }
        public string Bucket { get; init; }
    }
}