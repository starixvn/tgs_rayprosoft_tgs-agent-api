using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

#nullable enable

namespace TGS.Services.BackOffice.Application.Commands.BatchAdjust;

internal class ReadBatchAdjustItemsQueryService : IQueryService<ReadBatchAdjustItemsQuery, BatchAdjustBalanceItem>
{
    public async IAsyncEnumerable<BatchAdjustBalanceItem> QueryAsync(
        ReadBatchAdjustItemsQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new XLWorkbook(query.ImportFile);

        var sheet = reader.Worksheet(1);

        var index = 2;

        while (index < 152)
        {
            var userName = sheet.Cell(index, 1).GetFormattedString();

            if (string.IsNullOrWhiteSpace(userName))
                break;

            var adjustAmount = sheet.Cell(index, 2).GetValue<decimal>();
            var turnover = sheet.Cell(index, 3).GetValue<decimal>();
            var remark = sheet.Cell(index, 4).GetFormattedString();

            var item = new BatchAdjustBalanceItem(userName, adjustAmount, turnover, remark);

            yield return item;

            index++;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
