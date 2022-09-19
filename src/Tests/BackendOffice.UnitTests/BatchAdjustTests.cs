using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.PlayerService.Client;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Services.BatchAdjust;
using TGS.Services.BackOffice.Domain;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Tests.BackendOffice.UnitTests;

public class BatchAdjustTests
{
    [Test]
    public async Task Export_Form_Template()
    {
        var sut = new ExportBatchAdjustFormTemplateCommandService();

        var command = new ExportBatchAdjustFormTemplateCommand();

        var actual = await sut.GetAsync(command);

        Assert.IsNotNull(actual);
    }

    [Test]
    public async Task Get_Batch_Result()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();

        var sut = new ProcessResultQueryService(fakeBatchTempRepository);

        var query = new ProcessResultQuery(Guid.NewGuid());

        await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeBatchTempRepository.Received(1)
            .GetAdjustBalanceItemResultsAsync(Arg.Is(query.BatchId));
    }

    [Test]
    public async Task Preview_Batch()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();

        var sut = new PreviewBatchQueryService(fakeBatchTempRepository);

        var query = new PreviewBatchQuery(Guid.NewGuid());

        await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeBatchTempRepository.Received(1)
            .GetAdjustBalanceBatchAsync(Arg.Is(query.BatchId));
    }

    [Test]
    public async Task Process_Batch_AdjustAmount()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

        var sut = new ProcessBatchCommandService(
            fakeBatchTempRepository,
            fakeCashServiceClient,
            fakePlayerBucketRepository,
            NullLogger<ProcessBatchCommandService>.Instance);

        var command = new ProcessBatchCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            PlayerAdjustmentTransactionType.CompanyAdjust,
            Guid.NewGuid());

        _ = fakeBatchTempRepository.GetAdjustBalanceBatchAsync(Arg.Is(command.BatchId))
            .Returns(
                new[] {
                    new BatchAdjustBalanceItem(
                        1,
                        Guid.NewGuid(),
                        "test1",
                        "test1",
                        100,
                        0,
                        null)
                }.ToAsyncEnumerable());

        await sut.ExecuteAsync(command);

        _ = fakeCashServiceClient.Received(1)
            .AdjustCompanyAsync(
                Arg.Any<Guid>(),
                Arg.Is(100M),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is(command.ModifyUserId));
    }

    [Test]
    public async Task Process_Batch_By_PlayerAdjustmentTransactionType()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

        var sut = new ProcessBatchCommandService(
            fakeBatchTempRepository,
            fakeCashServiceClient,
            fakePlayerBucketRepository,
            NullLogger<ProcessBatchCommandService>.Instance);

        var command = new ProcessBatchCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            PlayerAdjustmentTransactionType.BonusAdjust,
            Guid.NewGuid());

        _ = fakeBatchTempRepository.GetAdjustBalanceBatchAsync(Arg.Is(command.BatchId))
            .Returns(
                new[] {
                    new BatchAdjustBalanceItem(
                        1,
                        Guid.NewGuid(),
                        "test1",
                        "test1",
                        100,
                        0,
                        null)
                }.ToAsyncEnumerable());

        await sut.ExecuteAsync(command);

        _ = fakeCashServiceClient.Received(1)
            .AdjustBonusAsync(
                Arg.Any<Guid>(),
                Arg.Is(100M),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is(command.ModifyUserId));
    }

    [Test]
    public async Task Process_Batch_TurnOver()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

        var sut = new ProcessBatchCommandService(
            fakeBatchTempRepository,
            fakeCashServiceClient,
            fakePlayerBucketRepository,
            NullLogger<ProcessBatchCommandService>.Instance);

        var command = new ProcessBatchCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            PlayerAdjustmentTransactionType.CompanyAdjust,
            Guid.NewGuid());

        var items = new[] {
            new BatchAdjustBalanceItem(
                1,
                Guid.NewGuid(),
                "test1",
                "test1",
                0,
                100,
                null)
        };
        _ = fakeBatchTempRepository.GetAdjustBalanceBatchAsync(Arg.Is(command.BatchId))
            .Returns(
                items.ToAsyncEnumerable());

        await sut.ExecuteAsync(command);

        _ = fakePlayerBucketRepository.Received(1)
            .CreatePlayerBucketsAsync(Arg.Is<CreatePlayerBucketRequest[]>(x => Enumerable.SequenceEqual(
                x,
                items.Select(item => new CreatePlayerBucketRequest(
                    command.BrandId,
                    item.PlayerCode,
                    item.WalletId,
                    BucketType.Adjustment,
                    item.Turnover)).ToArray())));
    }

    [Test]
    public async Task Read_File_Data()
    {
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        var sut = new ReadBatchAdjustItemsQueryService(fakePlayerServiceClient);

        var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        var fileInfo = fileProvider.GetFileInfo("/Assets/TestBatchAdjust.xlsx");

        await using var fileStream = fileInfo.CreateReadStream();

        var query = new ReadBatchAdjustItemsQuery(Guid.NewGuid(), fileStream);

        _ = fakePlayerServiceClient.GetPlayerInfoByUserNameAsync(Arg.Is(query.BrandId), Arg.Any<string>())
            .Returns(new PlayerInfo
            {
                PlayerCode = "test",
                BrandId = query.BrandId
            });

        _ = fakePlayerServiceClient.GetPlayerPropertiesAsync(Arg.Is("test"))
            .Returns(new Dictionary<string, string>
            {
                ["DefaultWalletId"] = Guid.NewGuid().ToString()
            }.ToImmutableDictionary());

        var actual = await sut.QueryAsync(query).ToArrayAsync();

        Assert.AreEqual(150, actual.Length);

        var lastRow = actual.Last();

        Assert.AreEqual(150, lastRow.AdjustAmount);
        Assert.AreEqual(150, lastRow.Turnover);
    }

    [Test]
    public async Task Save_Batch()
    {
        var fakeBatchTempRepository = Substitute.For<IBatchTempRepository>();

        var sut = new SaveBatchCommandService(fakeBatchTempRepository);

        var command = new SaveBatchCommand(Array.Empty<BatchAdjustBalanceItem>().ToAsyncEnumerable());

        _ = await sut.GetAsync(command);

        _ = fakeBatchTempRepository.Received(1)
            .SaveAdjustBalanceBatchAsync(Arg.Any<IEnumerable<BatchAdjustBalanceItem>>());
    }
}
