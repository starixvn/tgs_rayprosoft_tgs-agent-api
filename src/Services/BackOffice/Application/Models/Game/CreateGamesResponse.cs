using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Game
{
    public class CreateGamesResponse : MessageResponse
    {
        public string[] DuplicateIds { get; set; }
    }
}