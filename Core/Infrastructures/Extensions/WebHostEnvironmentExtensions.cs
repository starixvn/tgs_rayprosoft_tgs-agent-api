using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class WebHostEnvironmentExtensions
    {
        public static bool IsDevelopmentOrDocker(this IWebHostEnvironment env) => 
            env.IsDevelopment() || env.IsDocker();

        public static bool IsDocker(this IWebHostEnvironment env) =>
            env.IsEnvironment("Docker");
    }
}
