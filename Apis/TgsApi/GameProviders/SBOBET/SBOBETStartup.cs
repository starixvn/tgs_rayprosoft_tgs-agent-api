using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TGS.Apis.TgsApi.Commons;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET
{
    public sealed class SBOBETStartup : BaseStartup
    {
        protected override string LogPrefix => "sbo";

        public SBOBETStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {

        }
    }
}
