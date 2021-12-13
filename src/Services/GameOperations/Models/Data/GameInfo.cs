using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models.Data
{
    public record GameInfo(
        int Id,
        string ExternalId,
        GameStatus Status);
}
