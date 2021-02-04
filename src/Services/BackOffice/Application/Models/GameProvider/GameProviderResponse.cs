using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class GameProviderResponse : MessageResponse
    {
        public string[] DuplicateGameExternalIds { get; set; }
        public int[] InUseGameIds { get; set; }
    }
}