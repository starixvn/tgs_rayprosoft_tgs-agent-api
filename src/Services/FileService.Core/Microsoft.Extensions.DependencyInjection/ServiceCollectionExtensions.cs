using TGS.FileService;
using TGS.FileService.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static FileServiceConfiguration AddFileServiceCore(this IServiceCollection services)
        {
            services
                .AddOptions<AwsOptions>()
                .Services
                .AddTransient(typeof(IFileManager<>), typeof(FileManager<>));

            var configuration = new FileServiceConfiguration(services);

            return configuration;
        }
    }
}