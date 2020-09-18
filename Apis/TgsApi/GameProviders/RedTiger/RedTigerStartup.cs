using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TGS.Apis.TgsApi.Commons;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger
{
    public sealed class RedTigerStartup : BaseStartup
    {
        protected override string LogPrefix => "redtiger";

        public RedTigerStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {

        }
    }
}
