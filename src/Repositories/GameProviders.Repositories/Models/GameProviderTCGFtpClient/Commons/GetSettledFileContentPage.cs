using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient.Commons
{
    public record GetSettledFileContentPage
    {
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; init; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; init; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
