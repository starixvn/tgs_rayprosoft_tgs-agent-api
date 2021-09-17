using Microsoft.AspNetCore.Mvc;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record EntranceRequest
    {
        [ModelBinder(Name = "key")]
        public string Key { get; init; }

        [ModelBinder(Name = "message")]
        public string Message { get; init; }
    }
}