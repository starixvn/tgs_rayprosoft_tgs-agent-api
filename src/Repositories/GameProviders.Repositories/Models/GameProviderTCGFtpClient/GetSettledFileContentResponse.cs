using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient.Commons;

namespace TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient
{
    public record GetSettledFileContentResponse
    {
        public GetSettledFileContentData[] list { get; set; }
        public GetSettledFileContentPage page { get; set; }
    }
}
