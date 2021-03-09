using System.Linq;
using Valhalla.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorServiceCollectionExtensions
    {
        public static IServiceCollection AddOperatorApiCore(this IServiceCollection services)
        {
            var allTypes = typeof(OperatorServiceCollectionExtensions).Assembly.GetTypes();

            var commandTypeQuery = from type in allTypes
                                   let commandServiceInterface = type.GetInterfaces()
                                    .Where(x => x.IsGenericType
                                        && x.GetGenericTypeDefinition() == typeof(ICommandService<>))
                                    .FirstOrDefault()
                                   where commandServiceInterface != null
                                   select (ServiceType: commandServiceInterface, ImplementationType: type);

            foreach (var t in commandTypeQuery)
                services.AddTransient(t.ServiceType, t.ImplementationType);

            var getServiceTypeQuery = from type in allTypes
                                      let getServiceInterfaceType = type.GetInterfaces()
                                       .Where(x => x.IsGenericType
                                           && x.GetGenericTypeDefinition() == typeof(IGetService<,>))
                                       .FirstOrDefault()
                                      where getServiceInterfaceType != null
                                      select (ServiceType: getServiceInterfaceType, ImplementationType: type);

            foreach (var t in getServiceTypeQuery)
                services.AddTransient(t.ServiceType, t.ImplementationType);

            var queryServiceTypeQuery = from type in allTypes
                                        let queryServiceInterfaceType = type.GetInterfaces()
                                         .Where(x => x.IsGenericType
                                             && x.GetGenericTypeDefinition() == typeof(IQueryService<,>))
                                         .FirstOrDefault()
                                        where queryServiceInterfaceType != null
                                        select (ServiceType: queryServiceInterfaceType, ImplementationType: type);

            foreach (var t in queryServiceTypeQuery)
                services.AddTransient(t.ServiceType, t.ImplementationType);

            return services;
        }
    }
}
