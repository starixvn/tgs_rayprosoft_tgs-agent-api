using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Paging;

namespace TGS.Services.Operator
{
    public interface IStationLetterRepository
    {
        ValueTask<int> QueryStationLettersTotalCountAsync(string playerCode);

        IAsyncEnumerable<StationLetter> QueryStationLettersAsync(string playerCode, Paging<StationLetter> paging);

        IAsyncEnumerable<StationLetter> QueryStationLettersAsync(string playerCode);

        ValueTask SetStationLetterHaveReadAsync(string playerCode, Guid letterId);
    }
}
