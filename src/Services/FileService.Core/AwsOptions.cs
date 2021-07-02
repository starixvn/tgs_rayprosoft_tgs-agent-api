#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。

namespace TGS.FileService
{
    public class AwsOptions
    {
        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string Region { get; set; }

        public string ExternalPublicServiceUrl { get; set; }
        public string ExternalPrivateServiceUrl { get; set; }

        public string PublicBucket { get; set; }
        public string PrivateBucket { get; set; }
    }
}

#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。