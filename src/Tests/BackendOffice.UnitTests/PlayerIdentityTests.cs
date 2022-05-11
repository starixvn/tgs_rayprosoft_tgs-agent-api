using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
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
        public async Task Can_Count_Player_Identity_Forms_Query()
        {
            var fakeBackOfficeRepository = Substitute.For<IBackOfficeRepository>();

            var sut = new GetPlayerIdentityAuthenticationListQueryService(fakeBackOfficeRepository);

            var paging = new Paging<PlayerIdentityAuthenticationInfo>
            {
                RowCount = 10,
                Page = 0
            };

            paging.SortByPropertyName(nameof(PlayerIdentityAuthenticationInfo.CreatedOn), false);

            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");

            var query = new GetPlayerIdentityAuthenticationFormsQuery(
                brandId,
                null,
                null,
                null,
                null,
                null,
                null,
                paging);

            fakeBackOfficeRepository.QueryPlayerIdentityAuthenticationsAsync(
                Arg.Is(brandId),
                null,
                null,
                null,
                null,
                null,
                null)
                .Returns(new[]
                {
                    new PlayerIdentityAuthenticationInfo(
                        "",
                        "IdNumber",
                        "RealName",
                        "Remark",
                        PlayerIdentityReviewState.WaitingVerify,
                        "Username",
                        DateTimeOffset.UtcNow,
                        Guid.NewGuid()),
                    new PlayerIdentityAuthenticationInfo(
                        "",
                        "IdNumber1",
                        "RealName1",
                        "Remark1",
                        PlayerIdentityReviewState.Approve,
                        "Username1",
                        DateTimeOffset.UtcNow,
                        Guid.NewGuid())
                }.ToAsyncEnumerable());

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(2, actual);
        }

        [Test]
        public async Task Can_Query_Player_Identity_Forms()
        {
            var fakeBackOfficeRepository = Substitute.For<IBackOfficeRepository>();

            var sut = new GetPlayerIdentityAuthenticationListQueryService(fakeBackOfficeRepository);

            var paging = new Paging<PlayerIdentityAuthenticationInfo>
            {
                RowCount = 10,
                Page = 0
            };

            paging.SortByPropertyName(nameof(PlayerIdentityAuthenticationInfo.CreatedOn), false);

            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");

            var query = new GetPlayerIdentityAuthenticationFormsQuery(
                brandId,
                null,
                null,
                null,
                null,
                null,
                null,
                paging);

            fakeBackOfficeRepository.QueryPlayerIdentityAuthenticationsAsync(
                Arg.Is(brandId),
                null,
                null,
                null,
                null,
                null,
                null)
                .Returns(new[]
                {
                    new PlayerIdentityAuthenticationInfo(
                        "",
                        "IdNumber",
                        "RealName",
                        "Remark",
                        PlayerIdentityReviewState.WaitingVerify,
                        "Username",
                        DateTimeOffset.UtcNow,
                        Guid.NewGuid()),
                    new PlayerIdentityAuthenticationInfo(
                        "",
                        "IdNumber1",
                        "RealName1",
                        "Remark1",
                        PlayerIdentityReviewState.Approve,
                        "Username1",
                        DateTimeOffset.UtcNow,
                        Guid.NewGuid())
                }.ToAsyncEnumerable());

            await sut.QueryAsync(query).ToArrayAsync();

            _ = fakeBackOfficeRepository
                .Received(1)
                .QueryPlayerIdentityAuthenticationsAsync(
                    Arg.Any<Guid>(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);
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
    }
}
