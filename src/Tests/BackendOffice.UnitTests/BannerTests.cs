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
                    It.IsAny<BannerModifyInfo>(),
                    It.Is<string>(x => x == command.Modifier)),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Review_Banner()
        {
            var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

            var sut = new ReviewBannerCommandService(moqBackOfficeRepository.Object);

            var command = new ReviewBannerCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                BannerReviewState.Approve,
                null,
                "User1");

            await sut.ExecuteAsync(command);

            moqBackOfficeRepository.Verify(
                fake => fake.ReviewBannerAsync(
                    It.Is<Guid>(x => x == command.BannerId),
                    It.Is<BannerReviewState>(x => x == command.ReviewState),
                    It.Is<string>(x => x == command.Remark),
                    It.Is<string>(x => x == command.Reviewer)),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Set_Enable_Banner()
        {
            var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

            var sut = new SetIsEanbleBannerCommandService(moqBackOfficeRepository.Object);

            var command = new SetIsEanbleBannerCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                true,
                "User1");

            await sut.ExecuteAsync(command);

            moqBackOfficeRepository.Verify(
                fake => fake.SetIsEnableBannerAsync(
                    It.Is<Guid>(x => x == command.BannerId),
                    It.Is<bool>(x => x == command.IsEnable),
                    It.Is<string>(x => x == command.Modifier)),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Update_Banner_Data()
        {
            var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

            var sut = new UpdateBannerCommandService(moqBackOfficeRepository.Object);

            var command = new UpdateBannerCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "TestTitle",
                DateTimeOffset.Now,
                null,
                null,
                "User1");

            await sut.ExecuteAsync(command);

            moqBackOfficeRepository.Verify(
                fake => fake.UpdateBannerAsync(
                    It.Is<Guid>(x => x == command.BannerId),
                    It.IsAny<BannerModifyInfo>(),
                    It.Is<string>(x => x == command.Modifier)),
                Times.Exactly(1));
        }
    }
}
