namespace TGS.Apis.FileService
{
    public class AwsSettings
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string Region { get; set; }
        public string ServiceUrl { get; set; }
        public S3 S3 { get; set; }
        public CloudFront CloudFront { get; set; }
        public CloudWatch CloudWatch { get; set; }
    }

    public class S3
    {
        public string ServiceUrl { get; set; }
        public string Bucket { get; set; }
    }

    public class CloudFront
    {
        public string Url { get; set; }
    }

    public class CloudWatch
    {
        public string ServiceUrl { get; set; }
        public string GroupName { get; set; }
    }
}