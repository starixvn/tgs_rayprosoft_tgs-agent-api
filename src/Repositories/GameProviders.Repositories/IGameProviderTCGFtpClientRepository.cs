using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderTCGFtpClientRepository
    {
        Task<GetSettledFileContentResponse> GetSettledFileContentAsync(string path, string merchant, string directory, string file);

        Task<string[]> GetSettledFileListAsync(string path, string merchant, string directory);
    }
}
