using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class WebHostEnvironmentExtensions
    {
        public static bool IsDevelopmentOrDocker(this IWebHostEnvironment env)
        { 
            return env.IsDevelopment() || env.IsEnvironment("Docker");
        }
    }
}
