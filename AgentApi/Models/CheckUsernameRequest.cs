using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.AgentApi.Models;

public record CheckUsernameRequest
{
    [BindProperty(Name = "username", SupportsGet = true)]
    public string Username { get; init; }
}
