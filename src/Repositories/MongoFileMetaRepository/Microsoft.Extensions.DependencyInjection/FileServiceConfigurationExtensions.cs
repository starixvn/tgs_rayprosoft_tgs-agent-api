using System;
using TGS.FileService.Configuration;
using TGS.FileService.Repositories;
using TGS.FileService.Repositories.Model;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileServiceConfigurationExtensions
    {
        public static FileServiceConfiguration UseMongoRepository(this FileServiceConfiguration configuration, Action<FileMetaOptions, IServiceProvider> configure)
        {
            configuration.Services
                .AddOptions<FileMetaOptions>()
                .Configure(configure)
                .Services
                .AddTransient<IMongoContext<File>, MongoContext<File>>();

            return configuration
                .UseRepository<MongoFileMetaRepository>();
        }
    }
}
