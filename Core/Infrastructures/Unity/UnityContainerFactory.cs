using System;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace TGS.Infrastructures.Unity
{
    public class UnityContainerFactory<T> : IServiceProviderFactory<IUnityContainer> where T : BaseFactory
    {
        private IServiceCollection Services { get; set; }

        IUnityContainer IServiceProviderFactory<IUnityContainer>.CreateBuilder(IServiceCollection services)
        {
            Services = services;
            var factory = (T)Activator.CreateInstance(typeof(T));
            return factory?.Container;
        }

        IServiceProvider IServiceProviderFactory<IUnityContainer>.CreateServiceProvider(IUnityContainer containerBuilder)
        {
            return containerBuilder.BuildServiceProvider(Services);
        }
    }
}