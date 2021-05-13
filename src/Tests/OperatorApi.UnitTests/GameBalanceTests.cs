using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Operator.GameBalance;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameBalanceTests
    {
        [Test]
        public async Task Player_Can_Get_Transfer_Wallet_Balance()
        {
            // arrange
            var playerCode = "playerCode";
            var gameProviderCode = "KY";
            var fakeUri = new Uri("https://www.fake.com");

            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();

            var request = new GameProviderWalletGetBalanceRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode
            };

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);
            fakePluggalbeGameProviderApiClient
                .GetBalanceAsync(fakeUri, request)
                .Returns(new GameProviderWalletGetBalanceResponse() { Balance = 1000 });

            var sut = new GetGameBalanceQueryService(
                fakePluggalbeGameProviderApiClient,
                fakePluggableOperations);

            // act            
            var query = new GetGameBalanceQuery(playerCode, gameProviderCode);
            await sut.GetAsync(query, default);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .GetBalanceAsync(Arg.Is(fakeUri), Arg.Is(request));
        }

        [Test]
        public async Task Player_Can_Fund_Out()
        {
            // arrange
            var gameProviderCode = "KY";
            var playerCode = "playerCode";
            int amount = 10000;
            var fakeUri = new Uri("https://www.fake.com");

            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);
            var request = new GameProviderWalletFundRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode,
                Amount = amount
            };
            var sut = new SubmitGameBalanceFundOutCommandService(fakePluggalbeGameProviderApiClient, fakePluggableOperations);
            
            // act            
            var command = new SubmitGameBalanceFundOutCommand(gameProviderCode, playerCode, amount);
            await sut.ExecuteAsync(command, default);
            
            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .FundOutAsync(Arg.Is(fakeUri), Arg.Is(request));
        }

        [Test]
        public async Task Player_Can_Fund_In()
        {
            // arrange
            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();

            var gameProviderCode = "KY";
            var playerCode = "playerCode";
            var fakeUri = new Uri("https://www.fake.com");
            int amount = 10000;
            var request = new GameProviderWalletFundRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode,
                Amount = amount
            };
            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);
            var sut = new SubmitGameBalanceFundInCommandService(fakePluggalbeGameProviderApiClient, fakePluggableOperations);

            // act
            var command = new SubmitGameBalanceFundInCommand(gameProviderCode, playerCode, amount);
            await sut.ExecuteAsync(command, default);
            
            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .FundInAsync(Arg.Is(fakeUri), Arg.Is(request));
        }
    }
}