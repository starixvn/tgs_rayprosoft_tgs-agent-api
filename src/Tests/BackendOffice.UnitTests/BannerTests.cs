using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class BannerTests
    {
        [Test]
        public async Task User_Can_Create_Banner_Data()
        {
            var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

            var sut = new CreateBannerCommandService(moqBackOfficeRepository.Object);

            var command = new CreateBannerCommand(
                Guid.NewGuid(),
                "TestTitle",
                DateTimeOffset.Now,
                null,
                null,
                "User1");

            await sut.ExecuteAsync(command);

            moqBackOfficeRepository.Verify(
                fake => fake.CreateBannerAsync(
                    It.IsAny<BannerCreateInfo>(),
                    It.Is<string>(x => x == command.Modifier)),
                Times.Exactly(1));
        }
    }
}
