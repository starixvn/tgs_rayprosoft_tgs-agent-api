using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Withdraw;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class WithdrawFormTests
    {
        [Test]
        public async Task Manager_Can_Get_The_WithdrawForm_Detail()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new GetWithdrawFormDetailQueryService(moqPaymentServiceClient.Object);

            var formId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var query = new GetWithdrawFormDetailQuery(formId, brandId);

            await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetWithdrawFormAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.Is<Guid>(x => x == formId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Review_WithdrawForm()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var moqWalletExchangeService = new Mock<IWalletExchangeService>();

            var sut = new ReviewWithdrawFormCommandService(
                moqPaymentServiceClient.Object,
                moqPaymentClient.Object,
                moqWalletExchangeService.Object);

            var formId = Guid.NewGuid();
            var brandPaymentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var command = new ReviewWithdrawFormCommand(
                brandId,
                formId,
                brandPaymentId,
                ReviewState.Approve,
                null);

            moqPaymentServiceClient
                .Setup(fake => fake.GetWithdrawFormAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.Is<Guid>(x => x == formId)))
                .Returns(ValueTask.FromResult(
                    new WithdrawFormInfo
                    {
                        Id = formId,
                        BrandId = brandId,
                        Status = PaymentFlowState.WaitApprove
                    }));

            await sut.ExecuteAsync(command);

            moqPaymentServiceClient.Verify(
                fake => fake.ReviewWithdrawFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<Guid>(x => x == brandPaymentId),
                    It.Is<ReviewState>(x => x == ReviewState.Approve),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_List_WithdrawFormAsync()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new WithdrawFormQueryService(moqPaymentServiceClient.Object);

            var query = new WithdrawFormQuery(
                Guid.NewGuid(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new PageableRequest());

            _ = await sut.GetAsync(query, default);

            moqPaymentServiceClient.Verify(
                fake => fake.QueryWithdrawFormsAsync(
                    It.IsAny<WithdrawFormQueryConditions>(),
                    It.IsAny<Tgs.Payment.Clients.Paging>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Wallet_Exchange_By_Agent_Should_Call_AgentWalletRepository()
        {
            var moqPlayerService = new Mock<IPlayerService>();
            var moqAgentService = new Mock<IAgentService>();
            var moqWalletRepository = new Mock<IWalletRepository>();
            var moqAgentWalletRepository = new Mock<IAgentWalletRepository>();

            var sut = new WalletExchangeService(
                moqPlayerService.Object,
                moqAgentService.Object,
                moqWalletRepository.Object,
                moqAgentWalletRepository.Object);

            var brandId = Guid.NewGuid();
            var userType = UserType.Agent;
            var agentId = Guid.NewGuid();
            var transactionType = TransactionTypes.CancelWithdrawal;
            var amount = 1000M;

            moqAgentService
                .Setup(fake => fake.GetAgentAsync(
                    It.IsAny<Expression<Func<Agent, bool>>>(),
                    It.IsAny<Func<IQueryable<Agent>, IQueryable<Agent>>>()))
                .Returns(Task.FromResult(new Agent
                {
                    Id = agentId,
                    DefaultWalletId = Guid.NewGuid()
                }));

            await sut.ExchangeAsync(
                brandId,
                userType,
                agentId.ToString(),
                transactionType,
                amount);

            moqAgentWalletRepository.Verify(
                fake => fake.ExchangeAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.IsAny<Guid>(),
                    It.Is<Guid>(x => x == agentId),
                    It.Is<TransactionTypes>(x => x == transactionType),
                    It.Is<decimal>(x => x == amount),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Wallet_Exchange_By_Player_Should_Call_WalletRepository()
        {
            var moqPlayerService = new Mock<IPlayerService>();
            var moqAgentService = new Mock<IAgentService>();
            var moqWalletRepository = new Mock<IWalletRepository>();
            var moqAgentWalletRepository = new Mock<IAgentWalletRepository>();

            var sut = new WalletExchangeService(
                moqPlayerService.Object,
                moqAgentService.Object,
                moqWalletRepository.Object,
                moqAgentWalletRepository.Object);

            var brandId = Guid.NewGuid();
            var userType = UserType.Player;
            var playerCode = "ThisIsPlayerCode";
            var transactionType = TransactionTypes.CancelWithdrawal;
            var amount = 1000M;

            moqPlayerService
                .Setup(fake => fake.GetPlayerAsync(
                    It.IsAny<Expression<Func<Player, bool>>>(),
                    It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(new Player
                {
                    PlayerCode = playerCode,
                    DefaultWalletId = Guid.NewGuid()
                }));

            await sut.ExchangeAsync(
                brandId,
                userType,
                playerCode,
                transactionType,
                amount);

            moqWalletRepository.Verify(
                fake => fake.ExchangeAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.IsAny<Guid>(),
                    It.Is<string>(x => x == playerCode),
                    It.Is<TransactionTypes>(x => x == transactionType),
                    It.Is<decimal>(x => x == amount),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }
    }
}
