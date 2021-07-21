using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Services.Operations;
using TGS.Services.Operator.Services.Withdraw;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerWithdrawTests
    {
        [Test]
        public async Task Get_Player_Withdraw_Info_Should_Success()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new GetWithdrawFormsQueryService(fakePlayerOperations, fakePaymentServiceClient);

            fakePlayerOperations
                .GetPlayerByCodeAsync("playerCode")
                .Returns(new Player() { Username = "userName" });

            var query = new GetWithdrawFormsQuery(
                BrandId: Guid.NewGuid(),
                "playerCode",
                new List<PaymentFlowState>(),
                CreateOnRange: null,
                UpdateOnRange: null,
                new PageableRequest()
                );

            await sut.GetAsync(query, default);

            _ = fakePaymentServiceClient
                .Received(1)
                .QueryWithdrawFormsAsync(
                    Arg.Any<WithdrawFormQueryConditions>(),
                    Arg.Any<Tgs.Payment.Clients.Paging>());
        }
    }
}