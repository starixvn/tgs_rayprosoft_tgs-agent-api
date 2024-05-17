using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Models;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [Route("jwt")]
    public Task<AuthResponse> AuthenticateAsync(AuthRequest request) =>
        mediator.Send(request);
}