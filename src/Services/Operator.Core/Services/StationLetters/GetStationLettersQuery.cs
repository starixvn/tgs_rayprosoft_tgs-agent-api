using TGS.Core.Models.Messages;

namespace TGS.Services.Operator.Services.StationLetters
{
    public record GetStationLettersQuery(string PlayerCode, PageableRequest Paging);
}
