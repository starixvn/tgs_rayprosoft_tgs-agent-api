using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Infrastructure.ErrorMappers, it adds and resolves TgsErrorMapper as default;
        /// if you need to add custom error mappers, please DO implement 'customErrorMapperTypes' and 'get' parameters
        /// </summary>
        public static IServiceCollection AddErrorMappers(this IServiceCollection services, Type[] customErrorMapperTypes = null, Func<IServiceProvider, string, IErrorMapper> get = null)
        {
            customErrorMapperTypes ??= Array.Empty<Type>();
            var types = customErrorMapperTypes.Concat(new[] { typeof(TgsErrorMapper) }).ToArray();

            get ??= (provider, _) => provider.GetRequiredService<TgsErrorMapper>();

            return services.AddSingletonWithResolver(types, get);
        }

        public static ErrorMapperBuilder AddErrorMapper(this IServiceCollection services)
            => new ErrorMapperBuilder(services);
    }
}