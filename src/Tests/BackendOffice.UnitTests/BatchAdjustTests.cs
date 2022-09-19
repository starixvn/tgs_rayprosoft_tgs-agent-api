using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Commands.BatchAdjust;
using TGS.Services.BackOffice.Domain;
using System;
using NSubstitute;
using System.Collections.Generic;

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
    public async Task Read_File_Data()
    {
        var sut = new ReadBatchAdjustItemsQueryService();

        var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        var fileInfo = fileProvider.GetFileInfo("/Assets/TestBatchAdjust.xlsx");

        await using var fileStream = fileInfo.CreateReadStream();

        var query = new ReadBatchAdjustItemsQuery(fileStream);

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
