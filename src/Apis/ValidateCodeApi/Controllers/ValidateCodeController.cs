using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TGS.Services.ValidateCode;
using Valhalla.Services;

namespace ValidateCodeApi.Controllers
{
    /// <summary>
    /// ValidateCode Service
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("validate-code")]
    [ApiController]
    public class ValidateCodeController : ControllerBase
    {
        /// <summary>
        /// Save data by validate code.
        /// </summary>
        /// <param name="saveValidateDataService">The save validate data service.</param>
        /// <param name="validateCode">The validate code.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        [HttpPost("{validateCode}")]
        public async Task SetValidateCodeAsync(
            [FromServices] ICommandService<SaveValidateDataCommand> saveValidateDataService,
            string validateCode,
            [FromBody] JObject content)
        {
            await saveValidateDataService
                .ExecuteAsync(new SaveValidateDataCommand(validateCode, content.ToString()))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get data by validate code.
        /// </summary>
        /// <param name="getValidateDataService">The get validate data service.</param>
        /// <param name="validateCode">The validate code.</param>
        /// <returns></returns>
        [HttpGet("{validateCode}")]
        public async Task<JObject> GetValidateCodeDataAsync(
            [FromServices] IGetService<GetValidateDataQuery, string> getValidateDataService,
            string validateCode) =>
            JObject.Parse(
                await getValidateDataService
                    .GetAsync(new GetValidateDataQuery(validateCode))
                    .ConfigureAwait(false));
    }
}
