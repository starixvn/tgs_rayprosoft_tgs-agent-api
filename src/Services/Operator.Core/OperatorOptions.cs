using System;

namespace TGS.Services.Operator
{
    public class OperatorOptions
    {
        public Uri ValidateCodeBaseUrl { get; set; } = new Uri("https://localhost:5001");

        public Uri SmsBaseUrl { get; set; } = new Uri("https://localhost:5002");

        public Uri FileServiceBaseUrl { get; set; } = new Uri("");
    }
}
