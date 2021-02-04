using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class GetPlayersRequest : PageableRequest
    {
        public PlayerStatus? Status { get; set; }
        public Guid? VipId { get; set; }
        public string Username { get; set; }
    }
}
