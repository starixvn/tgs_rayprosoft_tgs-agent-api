using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TGS.Apis.TgsApi.Commons;

namespace TGS.Apis.TgsApi.GameProviders.Habanero
{
    public sealed class HabaneroStartup : BaseStartup
    {
        public HabaneroStartup(IConfiguration configuration, IWebHostEnvironment env) 
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "habanero";
    }
}
