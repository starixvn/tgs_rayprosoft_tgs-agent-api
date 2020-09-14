using System.Threading.Tasks;
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
    public class StartupTests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task Can_Starup()
        {
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            Mock<IConfiguration> configurationStub = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);


            IServiceCollection services = new ServiceCollection();


            var startup = new Startup(configurationStub.Object);





            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
            var s = webHost.Services.GetService<UserController>();

            s.Should().NotBe(null);
            //Assert.IsNotNull();



        }
    }
}
