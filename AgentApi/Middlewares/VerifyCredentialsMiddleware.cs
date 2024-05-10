using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.BrandCertificate;
using Tgs.BrandCertificate.Clients;
using TGS.Core.Models.Exceptions;

namespace TGS.Apis.AgentApi.Middlewares;

public sealed class VerifyCredentialsMiddleware(RequestDelegate next)
{
    private static readonly ActivitySource _VerifyCredentialsMiddlewareSource =
        new($"TGS.Apis.AgentApi.Middlewares.{nameof(VerifyCredentialsMiddleware)}");

    public async Task InvokeAsync(
        HttpContext context,
        IBrandServiceClient brandServiceClient)
    {
        using var activity = _VerifyCredentialsMiddlewareSource.StartActivity(nameof(VerifyCredentialsMiddleware));

        var brandId = context.GetBrandId();

        if (brandId is null) throw new InvalidCredentialsException("Brand doesn't exist");

        var brandStatus = await brandServiceClient.GetBrandStatusAsync(brandId.Value).ConfigureAwait(false);

        if (brandStatus is not (BrandStatus.Active or BrandStatus.Maintenance))
            throw new InvalidStatusException("Brand is inactive");

        context.Items.Add(Core.Models.Consts.ContextItems.BrandId, brandId);

        await next(context);
    }
}