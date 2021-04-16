using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TGS.UnitTests.Startups
{
    public abstract class StartupTestsBase
    {
        protected void ResolveAllRegistrations(IEnumerable<Type> resolveTypes, string namespacePrefix = "TGS")
        {
            var configuration = MockConfiguration();
            var envMock = new Mock<IWebHostEnvironment>();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddSingleton(envMock.Object);

            resolveTypes.ToList().ForEach(t => services.AddTransient(t));

            InitialStartupConfigureServices(services, configuration, envMock.Object);

            var sp = services.BuildServiceProvider();

            var error = new StringBuilder();

            foreach (var service in services)
            {
                var serviceType = service.ServiceType;

                try
                {
                    var @namespace = serviceType.Namespace ?? string.Empty;
                    if (@namespace.StartsWith(namespacePrefix, StringComparison.OrdinalIgnoreCase) is false)
                    {
                        continue;
                    }

                    if (serviceType.ContainsGenericParameters)
                    {
                        continue;
                    }

                    sp.GetRequiredService(serviceType);
                }
                catch (Exception e)
                {
                    error.AppendLine($"service type: {serviceType.FullName}, error: {e.Message}");
                }
            }

            if (error.Length > 0)
            {
                Console.WriteLine(error.ToString());
            }

            error.Length.Should().Be(0);
        }

        protected abstract void InitialStartupConfigureServices(IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment env);

        protected virtual IConfiguration MockConfiguration()
        {
            var configurationMock = new Mock<IConfiguration>();
            return configurationMock.Object;
        }
    }
}