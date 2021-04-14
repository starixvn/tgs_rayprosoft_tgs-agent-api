using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TGS.Apis.TgsApi.Commons;

namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public sealed class SabaStartup : BaseStartup
    {
        public SabaStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "saba";
    }
}