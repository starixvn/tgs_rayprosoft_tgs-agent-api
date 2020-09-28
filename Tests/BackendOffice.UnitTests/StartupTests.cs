using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Tests.BackendOffice.UnitTests.Controllers;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture, Parallelizable]
    public class StartupTests : BaseTests
    {

        [Test]
        public void Can_Run_Startup()
        {
            var configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["BackendOffice"])
                .Returns("server=localhost;database=BackendOffice;user=sa;password=1qaz2wsx!@");

            configurationSectionStub.Setup(x => x["Tgs"])
                .Returns("server=localhost;database=Tgs;user=sa;password=1qaz2wsx!@");

            var urls = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["Urls"])
                .Returns("nats://localhost:4222");

            var configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).
                Returns(configurationSectionStub.Object);

            configurationStub.Setup(x => x.GetSection("NatsSetting")).
                Returns(urls.Object);

            var envStub = new Mock<IWebHostEnvironment>();
            var services = new ServiceCollection();
            var startup = new Startup(configurationStub.Object, envStub.Object);

            startup.ConfigureServices(services);
            SetupHttpContext();

            services.AddSingleton(HttpContextAccessorMock.Object);

            var controllers = typeof(BaseContextController).Assembly.ExportedTypes
                .Where(x => !x.IsAbstract && typeof(ControllerBase).IsAssignableFrom(x)).ToList();

            controllers.ForEach(c => services.AddTransient(c));

            var serviceProvider = services.BuildServiceProvider();

            foreach (var controller in controllers.Select(controllerType => serviceProvider.GetRequiredService(controllerType)))
            {
                controller.Should().NotBe(null);
            }
        }


        private void SetupHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(p => p.User)
                .Returns(GetUserClaimsPrincipal());

            mockHttpContext.Setup(p => p.RequestServices)
                .Returns(SetupServiceProviderMock().Object);

            mockHttpContext.Setup(p => p.Connection)
                .Returns(new Mock<ConnectionInfo>().Object);

            var httpResponse = new Mock<HttpResponse>();
            httpResponse.Setup(p => p.Cookies)
                .Returns(new Mock<IResponseCookies>().Object);

            mockHttpContext.Setup(p => p.Response)
                .Returns(httpResponse.Object);

            HttpContextAccessorMock.Setup(p => p.HttpContext)
                .Returns(mockHttpContext.Object);
        }

        private Mock<IAuthenticationService> _authServiceMock;

        private Mock<IServiceProvider> SetupServiceProviderMock()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult(default(object)));

            _authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult(default(object)));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(_authServiceMock.Object);

            return serviceProviderMock;
        }
    }
}
