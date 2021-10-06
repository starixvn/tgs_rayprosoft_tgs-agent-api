using System;
using System.Security.Claims;

namespace TGS.Services.Operator.Services.Auth
{
    public record MakeBrandAuthTokenCommand(Guid BrandId, ClaimsIdentity Identity, string Site);
}
