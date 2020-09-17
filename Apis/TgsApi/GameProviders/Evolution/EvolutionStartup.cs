using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TGS.Apis.TgsApi.GameProviders.Evolution
{
    public sealed class EvolutionStartup : BaseStartup
    {
        protected override string LogPrefix => "evolution";

        public EvolutionStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {

        }
    }
}
