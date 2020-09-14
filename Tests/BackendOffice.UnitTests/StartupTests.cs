using BackendOffice.UnitTests.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice;
using TGS.Apis.BackendOffice.Controllers;

namespace BackendOffice.UnitTests
{
    [TestFixture, Parallelizable]
    public class StartupTests : BaseTests
    {

        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void Can_Starup()
        {
            var configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["BackendOffice"])
                .Returns("sqlserver connection string");

            var urls = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["Urls"])
                .Returns("nats://localhost:4222");

            var configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).
                Returns(configurationSectionStub.Object);

            configurationStub.Setup(x => x.GetSection("NatsSetting")).
                Returns(urls.Object);


            var services = new ServiceCollection();
            var startup = new Startup(configurationStub.Object);

            startup.ConfigureServices(services);
            services.AddSingleton(HttpContextAccessorMock.Object);



            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();

            Assert.IsNotNull(webHost);
            var s = webHost.Services.GetService<UserController>();

            s.Should().NotBe(null);
            //Assert.IsNotNull();



        }
    }
}
