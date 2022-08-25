#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.Promotion.Clients;
using Tgs.RouletteServices.Clients;
using TGS.Services.Operator.ApplicationServices.Promotions;
using RouletteResult = Tgs.DepositAccumulateRoulette.Clients.RouletteResult;
using PromotionRule = TGS.Services.Operator.ApplicationServices.Promotions.GetDepositAccumulateRouletteSettingQueryService.PromotionRule;

namespace TGS.Tests.OperatorApi.UnitTests;

public class PromotionTests
{
    [Test]
    public async Task Deposit_Accumulate_Roulette_Should_Get_Result()
    {
        var fakeDepositAccumulateRouletteClient = Substitute.For<IDepositAccumulateRouletteClient>();

        var command = new PlayDepositAccumulateRouletteCommand(Guid.NewGuid(), "player1");

        var expected = new RouletteResult(Guid.NewGuid(), 3, 5);

        _ = fakeDepositAccumulateRouletteClient
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var sut = new PlayDepositAccumulateRouletteCommandService(fakeDepositAccumulateRouletteClient);

        var actual = await sut.GetAsync(command);

        Assert.AreEqual(expected, actual);

        _ = fakeDepositAccumulateRouletteClient.Received(1)
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Get_Deposit_Accumulate_Roulette_Settings()
    {
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();
        var sut = new GetDepositAccumulateRouletteSettingQueryService(fakePromotionServiceClient);

        var campaignDetail = JsonDocument.Parse(
            "{\u0022depositItems\u0022: [{\u0022rouletteCount\u0022: 1, \u0022accumulateAmount\u0022: 1000}, {\u0022rouletteCount\u0022: 2, \u0022accumulateAmount\u0022: 2800}, {\u0022rouletteCount\u0022: 3, \u0022accumulateAmount\u0022: 4800}, {\u0022rouletteCount\u0022: 4, \u0022AccumulateAmount\u0022: 6600}, {\u0022rouletteCount\u0022: 6, \u0022accumulateAmount\u0022: 8800}], \u0022rouletteItems\u0022: [{\u0022id\u0022: \u00221cd30c0a-a61c-49fc-ba95-0ba78bfbcbb1\u0022, \u0022bonus\u0022: 0, \u0022title\u0022: \u0022iphone 13 pro\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0}, {\u0022id\u0022: \u0022cb2cad25-a580-4e8c-ae71-b6ddbf019e16\u0022, \u0022bonus\u0022: 0, \u0022title\u0022: \u0022100000PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0}, {\u0022id\u0022: \u00222d361c39-a0ea-4eb3-b8b2-466a7b09e11c\u0022, \u0022bonus\u0022: 0, \u0022title\u0022: \u002230000PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0}, {\u0022id\u0022: \u0022f099a2d4-c7ea-4b0f-ab4d-ed8dafc6fa55\u0022, \u0022bonus\u0022: 0, \u0022title\u0022: \u00228888PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0}, {\u0022id\u0022: \u002291c5e297-3676-43fa-be48-b2a7ae566adb\u0022, \u0022bonus\u0022: 1800, \u0022title\u0022: \u00221800PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.02}, {\u0022id\u0022: \u0022b094c17a-fb46-4cc2-99f2-8fceabcf6a62\u0022, \u0022bonus\u0022: 800, \u0022title\u0022: \u0022800PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.03}, {\u0022id\u0022: \u0022ab85fad0-fa30-46ef-9963-693fdbe8deb5\u0022, \u0022bonus\u0022: 300, \u0022title\u0022: \u0022300PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.04}, {\u0022id\u0022: \u002211aa0da0-ea5d-48e0-b979-08e05126841b\u0022, \u0022bonus\u0022: 120, \u0022title\u0022: \u0022120PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.06}, {\u0022id\u0022: \u00222313fa68-299d-408e-9ca2-2d3d9bb92711\u0022, \u0022bonus\u0022: 50, \u0022title\u0022: \u002250PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.07}, {\u0022id\u0022: \u002240ac9d47-24bb-4e74-8550-5456b47dbcab\u0022, \u0022bonus\u0022: 30, \u0022title\u0022: \u002230PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.2}, {\u0022id\u0022: \u002289d14b99-002d-4814-bf9c-020f94d2e75b\u0022, \u0022bonus\u0022: 18, \u0022title\u0022: \u002218PHP\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.3}, {\u0022id\u0022: \u002252e234ab-fb35-4673-8194-8c156e2874d1\u0022, \u0022bonus\u0022: 0, \u0022title\u0022: \u0022Thank You\u0022, \u0022turnoverRate\u0022: 3, \u0022winningProbability\u0022: 0.1}]}");

        var query = new GetDepositAccumulateRouletteSettingQuery(Guid.NewGuid());

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns(new PromotionMainInfo
            {
                Id = query.PromotionId,
                EventDuring = new During(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)),
                IsAutoSent = false,
                Name = "Test Promotion",
                PromotionType = "DepositAccumulateRoulette"
            });

        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns(new PromotionRule { CompaignDetail = campaignDetail });

        var actual = await sut.GetAsync(query);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual!.RouletteItems.Any());

        _ = fakePromotionServiceClient
            .Received(1)
            .GetPromotionRuleAsync<PromotionRule>(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Get_Deposit_Accumulate_Roulette_Settings_When_Promotion_Is_Not_Found()
    {
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();
        var sut = new GetDepositAccumulateRouletteSettingQueryService(fakePromotionServiceClient);

        var query = new GetDepositAccumulateRouletteSettingQuery(Guid.NewGuid());

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns((PromotionMainInfo?)null);

        var actual = await sut.GetAsync(query);

        Assert.IsNull(actual);

        _ = fakePromotionServiceClient
            .DidNotReceive()
            .GetPromotionRuleAsync<PromotionRule>(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public void Get_Deposit_Accumulate_Roulette_Settings_When_Promotion_Type_Is_Not_DepositAccumulateRoulette()
    {
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();
        var sut = new GetDepositAccumulateRouletteSettingQueryService(fakePromotionServiceClient);

        var query = new GetDepositAccumulateRouletteSettingQuery(Guid.NewGuid());

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns(new PromotionMainInfo
            {
                Id = query.PromotionId,
                EventDuring = new During(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)),
                IsAutoSent = false,
                Name = "Test Promotion",
                PromotionType = "Roulette"
            });

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(query));
    }

    [Test]
    public async Task Get_Deposit_Accumulate_Roulette_Settings_When_Rule_Is_Null()
    {
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();
        var sut = new GetDepositAccumulateRouletteSettingQueryService(fakePromotionServiceClient);

        var query = new GetDepositAccumulateRouletteSettingQuery(Guid.NewGuid());

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns(new PromotionMainInfo
            {
                Id = query.PromotionId,
                EventDuring = new During(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)),
                IsAutoSent = false,
                Name = "Test Promotion",
                PromotionType = "DepositAccumulateRoulette"
            });

        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(
                Arg.Is(query.PromotionId),
                Arg.Any<CancellationToken>())
            .Returns((PromotionRule?)null);

        var actual = await sut.GetAsync(query);

        Assert.IsNull(actual);
    }

    [Test]
    public async Task Get_Roulette_Record()
    {
        var fakeRouletteSignInClient = Substitute.For<IRouletteSignInClient>();
        var sut = new GetSignInRouletteRecordQueryService(fakeRouletteSignInClient);

        var query = new GetSignInRouletteRecordQuery("player1", Guid.NewGuid());

        await sut.GetAsync(query);

        _ = fakeRouletteSignInClient.Received(1)
            .GetRouletteRecordAsync(
                Arg.Is(query.PromotionId),
                Arg.Is(query.PlayerCode),
                Arg.Any<DateOnly>());
    }

    [Test]
    public async Task Roulette_Should_Get_Result()
    {
        var fakeRouletteSignInClient = Substitute.For<IRouletteSignInClient>();
        var sut = new SignInRouletteCommandService(fakeRouletteSignInClient);

        var command = new SignInRouletteCommand("player1", Guid.NewGuid());

        await sut.GetAsync(command);

        _ = fakeRouletteSignInClient.Received(1)
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<DateOnly>());
    }
}
