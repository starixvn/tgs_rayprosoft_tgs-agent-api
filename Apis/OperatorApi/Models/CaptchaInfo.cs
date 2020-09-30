namespace TGS.Apis.OperatorApi.Models
{
    public class CaptchaInfo
    {
        /// <summary>
        /// image of captcha
        /// </summary>
        public byte[] Image { get; init; }

        /// <summary>
        /// the value in captcha image, encoded with DES
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// image content type
        /// </summary>
        public string ContentType { get; set; }
    }
}
