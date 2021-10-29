using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Commands.Withdraw;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.WalletService;
using TGS.Services.BackOffice.Application.Services;

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
            var moqWalletExchangeService = new Mock<IWalletExchangeService>();

            var sut = new ReviewWithdrawFormCommandService(
                moqPaymentServiceClient.Object,
                moqWalletExchangeService.Object);

            var formId = Guid.NewGuid();
            var brandPaymentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var command = new ReviewWithdrawFormCommand(
                brandId,
                formId,
                brandPaymentId,
                ReviewState.Approve,
                null,
                "93a723b2-9ebb-4607-ee8d-08d978fba75f",
                "reviewUserName");

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
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Paid_Manual_WithdrawFormAsync()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new PaidWithdrawFormCommandService(moqPaymentServiceClient.Object);

            var formId = Guid.NewGuid();
            var brandPaymentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var command = new PaidWithdrawFormCommand(
                brandId,
                formId,
                "93a723b2-9ebb-4607-ee8d-08d978fba75f",
                "reviewUserName");

            moqPaymentServiceClient
                .Setup(fake => fake.GetWithdrawFormAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.Is<Guid>(x => x == formId)))
                .Returns(ValueTask.FromResult(
                    new WithdrawFormInfo
                    {
                        Id = formId,
                        BrandId = brandId,
                        Status = PaymentFlowState.Approved
                    }));

            await sut.ExecuteAsync(command);

            moqPaymentServiceClient.Verify(
                fake => fake.SubmitWithdrawFormStatusAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<PaymentFlowState>(x => x == PaymentFlowState.Paid),
                    It.IsAny<string>(),
                    It.IsAny<decimal?>(),
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
            var userId = Guid.NewGuid();

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
                amount,
                userId.ToString());

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
            var userId = Guid.NewGuid();

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
                amount,
                userId.ToString());

            moqWalletRepository.Verify(
                fake => fake.ExchangeAsync(
                    It.Is<Guid>(x => x == brandId),
                    It.IsAny<Guid>(),
                    It.Is<string>(x => x == playerCode),
                    It.Is<TransactionTypes>(x => x == transactionType),
                    It.Is<decimal>(x => x == amount),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_Untreated_WithdrawForms_Count()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new CountUntreatedWithdrawFormsQueryService(
                moqPaymentServiceClient.Object);

            var query = new CountUntreatedWithdrawFormsQuery(Guid.NewGuid());

            await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.CountUntreatedWithdrawFormsAsync(
                    It.Is<Guid>(x => x == query.BrandId)),
                Times.Exactly(1));
        }
    }
}