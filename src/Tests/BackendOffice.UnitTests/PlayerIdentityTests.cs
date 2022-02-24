using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PlayerIdentityTests
    {
        [Test]
        public async Task Approve_Player_Identity_Form_Should_Success()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();

            var formId = Guid.NewGuid();
            fakePlayerService
                .GetPlayerIdentityFormByIdAsync(formId)
                .Returns(new PlayerIdentityFormInfo
                {
                    FormId = formId,
                    Status = PlayerIdentityReviewState.WaitingVerify
                });

            var sut = new ReviewPlayerIdentityFormCommandService(fakePlayerService);

            var brandId = Guid.NewGuid();

            var command = new ReviewPlayerIdentityFormCommand(
                brandId,
                formId,
                PlayerIdentityReviewState.Approve,
                "remark",
                "reviewUserId",
                "reviewUserName");

            await sut.ExecuteAsync(command);

            _ = fakePlayerService
                .Received(1)
                .SignOffPlayerIdentityFormAsync(
                    brandId,
                    formId,
                    PlayerIdentityReviewState.Approve,
                    "reviewUserId",
                    "reviewUserName",
                    "remark");
        }

        [Test]
        public async Task Reject_Player_Identity_Form_Should_Success()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();

            var formId = Guid.NewGuid();
            fakePlayerService
                .GetPlayerIdentityFormByIdAsync(formId)
                .Returns(new PlayerIdentityFormInfo
                {
                    FormId = formId,
                    Status = PlayerIdentityReviewState.WaitingVerify
                });

            var sut = new ReviewPlayerIdentityFormCommandService(fakePlayerService);

            var brandId = Guid.NewGuid();

            var command = new ReviewPlayerIdentityFormCommand(
                brandId,
                formId,
                PlayerIdentityReviewState.Reject,
                "remark",
                "reviewUserId",
                "reviewUserName");

            await sut.ExecuteAsync(command);

            _ = fakePlayerService
                .Received(1)
                .SignOffPlayerIdentityFormAsync(
                    brandId,
                    formId,
                    PlayerIdentityReviewState.Reject,
                    "reviewUserId",
                    "reviewUserName",
                    "remark");
        }

        [Test]
        public async Task Approve_Player_Identity_Form_Status_Is_Reject_Should_Failed()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();

            var formId = Guid.NewGuid();
            fakePlayerService
                .GetPlayerIdentityFormByIdAsync(formId)
                .Returns(new PlayerIdentityFormInfo
                {
                    FormId = formId,
                    Status = PlayerIdentityReviewState.Reject
                });

            var sut = new ReviewPlayerIdentityFormCommandService(fakePlayerService);

            var brandId = Guid.NewGuid();

            var command = new ReviewPlayerIdentityFormCommand(
                brandId,
                formId,
                PlayerIdentityReviewState.Approve,
                "remark",
                "reviewUserId",
                "reviewUserName");

            await sut.ExecuteAsync(command);

            _ = fakePlayerService
                .Received(0)
                .SignOffPlayerIdentityFormAsync(
                    brandId,
                    formId,
                    PlayerIdentityReviewState.Approve,
                    "reviewUserId",
                    "reviewUserName",
                    "remark");
        }

        [Test]
        public async Task Get_Player_Identity_Form_Should_Success()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();

            fakePlayerService
                .GetPlayerIdentityFormAsync("playerCode")
                .Returns(new PlayerIdentityFormModel
                {
                    Id = Guid.NewGuid(),
                    Data = "{}"
                });

            var sut = new GetPlayerIdentityFormQueryService(fakePlayerService);

            _ = await sut.GetAsync(new GetPlayerIdentityFormQuery("playerCode"));

            _ = fakePlayerService.Received(1).GetPlayerIdentityFormAsync("playerCode");
        }
    }
}
