using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations.Models.Data
{
    internal record ProcessorData
    {
        public ITgsRepository Repository { get; init; }
        public BetData BetData { get; init; }
        public PlayerRelatedData Player { get; init; }
        public CachedGameProvider GameProvider { get; init; }
        public CachedGame[] Games { get; init; }
    }
}