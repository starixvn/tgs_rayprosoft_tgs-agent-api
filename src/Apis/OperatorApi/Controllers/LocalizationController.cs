using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using TGS.Services.Operator.Services.Localization;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly static PhysicalFileProvider _FileProvider =
            new PhysicalFileProvider(Environment.CurrentDirectory);

        [HttpGet]
        public ValueTask<IDictionary<string, string>> GetLocalizeDictionaryAsync(
            [FromServices] IGetService<GetAllLocalizationStringsQuery, IDictionary<string, string>> getService)
        {
            var currentCultureName = CultureInfo.CurrentUICulture.Name;

            var query = new GetAllLocalizationStringsQuery(currentCultureName);

            return getService.GetAsync(query);
        }
    }
}
