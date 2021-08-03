using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public class ErrorMapperBuilder
    {
        private ErrorMapperTypeCollection _mapperTypes;

        public IServiceCollection Services { get; }

        public ErrorMapperBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            var desc = services
                .Where(desc => desc.ServiceType == typeof(ErrorMapperTypeCollection))
                .FirstOrDefault();

            _mapperTypes = desc?.ImplementationInstance as ErrorMapperTypeCollection ?? new ErrorMapperTypeCollection();

            if (desc == null)
                services.AddSingleton(_mapperTypes);

            services.TryAddSingleton<IErrorMapper, CompositeErrorMapper>();
        }

        public ErrorMapperBuilder AddErrorMapper<TMapper>() where TMapper : IErrorMapper
        {
            _mapperTypes.Add(typeof(TMapper));

            return this;
        }
    }
}
