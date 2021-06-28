using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Tgs.Payment;
using Tgs.Payment.Clients;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
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
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new GetWithdrawFormDetailQueryService(moqPaymentRepository.Object);

            var formId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var query = new GetWithdrawFormDetailQuery(formId, brandId);

            await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetWithdrawFormByIdAsync(It.Is<Guid>(x => x == formId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Review_WithdrawForm()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var moqWithdrawProcessorFactory = new Mock<IWithdrawProcessorFactory>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var moqWalletExchangeService = new Mock<IWalletExchangeService>();

            var sut = new ReviewWithdrawFormCommandService(
                moqPaymentRepository.Object,
                moqWithdrawProcessorFactory.Object,
                moqPaymentClient.Object,
                moqWalletExchangeService.Object);

            var formId = Guid.NewGuid();
            var brandPaymentId = Guid.NewGuid();
            var command = new ReviewWithdrawFormCommand(
                formId,
                brandPaymentId,
                ReviewState.Approve,
                null);

            moqPaymentRepository
                .Setup(fake => fake.GetWithdrawFormByIdAsync(It.Is<Guid>(x => x == formId)))
                .Returns(ValueTask.FromResult(new WithdrawFormInfo(
                    formId,
                    "ThisIsPlayerCode",
                    UserType.Player,
                    "UserName",
                    Guid.NewGuid(),
                    1000M,
                    1000,
                    null,
                    Guid.NewGuid(),
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow)));

            moqPaymentRepository
                .Setup(fake => fake.GetBrandPaymentByIdAsync(It.Is<Guid>(x => x == brandPaymentId)))
                .Returns(ValueTask.FromResult(new BrandPaymentInfo(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "TestChannel",
                    "TestChannelCode",
                    "{}",
                    new ValueRange<decimal>
                    {
                        From = 0M,
                        To = 1000M
                    },
                    true,
                    false,
                    false,
                    new ValueRange<decimal>
                    {
                        From = 0M,
                        To = decimal.MaxValue
                    },
                    ConditionMatchType.MatchOne,
                    TransferType.Withdraw,
                    true)));

            moqPaymentRepository
                .Setup(fake => fake.GetPaymentAsync(It.IsAny<Guid>()))
                .Returns(ValueTask.FromResult(new PaymentInfo(Guid.NewGuid(), "LeePay")));

            var moqProcessor = new Mock<IWithdrawProcessor>();
            moqWithdrawProcessorFactory
                .Setup(fake => fake.GetWithdrawProcessor(It.Is<string>(x => x == "LeePay")))
                .Returns(moqProcessor.Object);

            await sut.ExecuteAsync(command);

            moqPaymentRepository.Verify(
                fake => fake.SignOffWithdrawFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<Guid>(x => x == brandPaymentId),
                    It.IsAny<string>(),
                    It.Is<ReviewState>(x => x == ReviewState.Approve),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_List_WithdrawFormAsync()
        {
            var jsonSerializationProvider = new JsonSerializationProvider();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.NewGuid();

            await tgsRepository.Players.AddAsync(
                new Player
                {
                    PlayerCode = "PlayerCode",
                    BrandId = brandId
                });

            var pmcId = Guid.NewGuid();
            await tgsRepository.AddAsync(
                new PaymentMethodChannel
                {
                    Id = pmcId,
                    PaymentId = Guid.NewGuid(),
                    MethodId = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid()
                });

            var brandPaymentId = Guid.NewGuid();
            await tgsRepository.BrandPayments.AddAsync(
                new BrandPayment
                {
                    Id = brandPaymentId,
                    BrandId = brandId,
                    PaymentMethodChannelId = pmcId
                });

            await tgsRepository.WithdrawForms.AddRangeAsync(
                new WithdrawForm
                {
                    Id = Guid.NewGuid(),
                    UserId = "PlayerCode",
                    UserName = "Player1",
                    UserType = 1,
                    BrandId = brandId,
                    Amount = 1000M,
                    Status = 1000,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                });

            await tgsRepository.SaveChangesAsync();

            var sut = new WithdrawFormQueryService(tgsRepository, jsonSerializationProvider);

            var query = new WithdrawFormQuery(
                brandId,
                null,
                null,
                null,
                null,
                null,
                null,
                new PageableRequest());

            var actual = await sut.QueryAsync(query).ToArrayAsync();

            Assert.IsTrue(actual.Length == 1);
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
    }
}
