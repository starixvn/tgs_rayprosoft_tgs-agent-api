using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddScopedWithResolver<TInterface>(this IServiceCollection services,
            Type[] implementTypes, Func<IServiceProvider, string, TInterface> resolver)
            where TInterface : class => 
            AddServicesWithResolver(services, implementTypes, resolver, ServiceLifetime.Scoped);

        public static IServiceCollection AddSingletonWithResolver<TInterface>(this IServiceCollection services,
            Type[] implementTypes, Func<IServiceProvider, string, TInterface> resolver)
            where TInterface : class =>
            AddServicesWithResolver(services, implementTypes, resolver, ServiceLifetime.Singleton);

        public static IServiceCollection AddTransientWithResolver<TInterface>(this IServiceCollection services,
            Type[] implementTypes, Func<IServiceProvider, string, TInterface> resolver)
            where TInterface : class =>
            AddServicesWithResolver(services, implementTypes, resolver, ServiceLifetime.Transient);

        private static IServiceCollection AddServicesWithResolver<TInterface>(IServiceCollection services,
            Type[] implementTypes, Func<IServiceProvider, string, TInterface> resolver, ServiceLifetime lifetime)
            where TInterface : class
        {
            foreach (var t in implementTypes)
            {
                if (t == null)
                {
                    continue;
                }

                if (!typeof(TInterface).IsAssignableFrom(t))
                {
                    throw new InvalidOperationException(
                        $"Cannot add service that is not implemented '{typeof(TInterface).Name}', error type: {t.Name}");
                }

                AddService(services, t, lifetime);
            }

            AddResolver(services, resolver, lifetime);

            return services;
        }

        private static void AddService(IServiceCollection services, Type type, ServiceLifetime lifetime) =>
            services.Add(new(type, type, lifetime));
        

        private static void AddResolver<TInterface>(IServiceCollection services,
            Func<IServiceProvider, string, TInterface> resolver, ServiceLifetime lifetime)
            where TInterface : class
        {
            Func<string, TInterface> Resolve(IServiceProvider provider) => key => resolver(provider, key);
            services.Add(new(typeof(Func<string, TInterface>), Resolve, lifetime));
        }
    }
}