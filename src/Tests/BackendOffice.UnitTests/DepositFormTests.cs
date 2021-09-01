using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TGS.Clients.Promotion.Client;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class DepositFormTests
    {
        [Test]
        public async Task Manager_Can_Get_The_DepositForm_Detail()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new GetDepositFormDetailQueryService(moqPaymentServiceClient.Object);

            var formId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var query = new GetDepositFormDetailQuery(formId, brandId);

            await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetDepositFormAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.Is<Guid>(x => x == formId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Review_DepositForm()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqPlayerService = new Mock<IPlayerService>();
            var moqPlayerBucketRepository = new Mock<IPlayerBucketRepository>();
            var moqPromotionClient = new Mock<IPromotionClient>();
            var logger = new Mock<ILogger<ReviewDepositFormCommandService>>();

            var sut = new ReviewDepositFormCommandService(
                moqPaymentServiceClient.Object,
                moqPlayerService.Object,
                moqPlayerBucketRepository.Object,
                moqPromotionClient.Object,
                logger.Object);

            var brandId = Guid.NewGuid();
            var formId = Guid.NewGuid();
            var walletId = Guid.NewGuid();
            var command = new ReviewDepositFormCommand(brandId, formId, ReviewState.Approve, null);

            moqPaymentServiceClient
                .Setup(fake => fake.GetDepositFormAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.Is<Guid>(x => x == formId)))
                .Returns(ValueTask.FromResult(new DepositFormInfo
                {
                    Id = Guid.NewGuid(),
                    PaymentMethodId = Guid.NewGuid(),
                    Amount = 1000M,
                    BrandId = brandId,
                    BrandPaymentId = Guid.NewGuid(),
                    ChannelName = "ChannelName",
                    CreatedOn = DateTimeOffset.Now,
                    PlayerCode = "ThisIsPlayerCode",
                    Status = PaymentFlowState.WaitApprove,
                    UpdatedOn = DateTimeOffset.Now,
                    UserName = "PlayerName"
                }));
            moqPlayerService
                .Setup(fake => fake.GetPlayerAsync(
                    It.IsAny<Expression<Func<Player, bool>>>(),
                    It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(new Player
                {
                    PlayerCode = "ThisIsPlayerCode",
                    DefaultWalletId = walletId
                }));

            await sut.ExecuteAsync(command);

            moqPaymentServiceClient.Verify(
                fake => fake.ReviewDepositFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<ReviewState>(x => x == ReviewState.Approve),
                    It.IsAny<string>()),
                Times.Exactly(1));

            moqPlayerBucketRepository.Verify(
                fake => fake.CreatePlayerBucketWithDepositAsync(
                    It.Is<CreatePlayerBucketWithDepositRequest>(
                        x => x.BrandId == brandId
                            && x.DefaultWalletId == walletId
                            && x.DepositAmount == 1000M
                            && x.PlayerCode == "ThisIsPlayerCode")),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_List_DepositForm()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new DepositFormQueryService(moqPaymentServiceClient.Object);

            var query = new DepositFormQuery(
                Guid.NewGuid(),
                null,
                null,
                null,
                null,
                null,
                null,
                new PageableRequest());

            _ = await sut.GetAsync(query, default);

            moqPaymentServiceClient.Verify(
                fake => fake.QueryDepositFormsAsync(
                    It.IsAny<DepositFormQueryConditions>(),
                    It.IsAny<Tgs.Payment.Clients.Paging>()),
                Times.Exactly(1));
        }
    }
}