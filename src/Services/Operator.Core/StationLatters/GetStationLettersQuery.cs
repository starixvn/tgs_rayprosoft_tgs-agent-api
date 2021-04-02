using TGS.Core.Models.Messages;

namespace TGS.Services.Operator.StationLetters
{
    public record GetStationLettersQuery(string PlayerCode, PageableRequest Paging);
}
