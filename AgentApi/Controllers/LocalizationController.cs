using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.Agent.ApplicationServices.Localization;
using Tgs.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocalizationController : ControllerBase
{
    [HttpGet]
    public ValueTask<IDictionary<string, string>> GetLocalizeDictionaryAsync(
        [FromServices] IGetService<GetAllLocalizationStringsQuery, IDictionary<string, string>> getService)
    {
        var currentCultureName = CultureInfo.CurrentUICulture.Name;

        var query = new GetAllLocalizationStringsQuery(currentCultureName);

        return getService.GetAsync(query);
    }
}