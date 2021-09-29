using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient.Commons;

namespace TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient
{
    public record GetSettledFileContentResponse
    {
        [JsonPropertyName("page")]
        public GetSettledFileContentPage Page { get; set; }
        [JsonPropertyName("list")]
        public GetSettledFileContentData[] Datas { get; set; }
    }
}
