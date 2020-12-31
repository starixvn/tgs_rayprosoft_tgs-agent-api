using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class UploadGamesResponse : MessageResponse
    {
        public UploadGameModel[] Games { get; set; }
    }
}