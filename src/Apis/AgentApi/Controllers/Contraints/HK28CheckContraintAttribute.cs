using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.AgentApi.Extensions;
using TGS.Services.Agent;

namespace TGS.Apis.AgentApi.Controllers.Contraints;

public class HK28CheckContraintAttribute : Attribute, IActionConstraint
{
    public int Order => 0;

    public bool Accept(ActionConstraintContext context)
    {
        var brandId = context.RouteContext.HttpContext.Items.GetBrandId();

        if (brandId == Guid.Empty)
            return false;

        var brandRepository = context.RouteContext.HttpContext.RequestServices.GetService<IBrandRepository>();

        if (brandRepository is null)
            return false;

        var brand = brandRepository.GetBrandInfoAsync(brandId).GetAwaiter().GetResult();

        if (brand is null)
            return false;

        return brand.Code is "HK28";
    }
}
