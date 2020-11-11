using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.File
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFile(this IServiceCollection services, IConfiguration configuration,
            string configSectionName = "FileSetting")
        {
            services.Configure<FileSetting>(configuration.GetSection(configSectionName));
            services.AddSingleton<IFileService, FileService>();

            return services;
        }
    }
}