using System;
using System.Text.Json;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;

namespace TGS.Apis.BackendOffice.Models.Player
{
    public class PlayerIdentityForm
    {
        public JsonDocument AuthData { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public Guid FormId { get; set; }

        public string IdNumber { get; set; }

        public string RealName { get; set; }

        public string Remark { get; set; }

        public DateTimeOffset? ReviewedOn { get; set; }

        public string ReviewedUserName { get; set; }

        public PlayerIdentityReviewState Status { get; set; }
    }
}
