using Microsoft.AspNetCore.Mvc;

namespace TGS.Shared.Game.Models.TF.Incoming;

public record GetClientBalanceRequest
{
    [FromQuery(Name = "loginName")]
    public string LoginName { get; set; }
}
