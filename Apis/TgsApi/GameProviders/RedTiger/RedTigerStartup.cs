using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons.Controllers;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers;

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
