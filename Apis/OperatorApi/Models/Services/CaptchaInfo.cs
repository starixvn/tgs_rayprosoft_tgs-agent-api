namespace TGS.Apis.OperatorApi.Models.Services
{
    public record CaptchaInfo
    {
        public byte[] Image { get; init; }
        public string Captcha { get; init; }
        public string ContentType { get; init; }
    }
}
