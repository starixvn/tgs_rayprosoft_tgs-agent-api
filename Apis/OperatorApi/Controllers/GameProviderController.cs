using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages.GameProviders;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using static TGS.Core.Models.Consts.LocalizationContext;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        private readonly ILicenseeCurrencyCodeMapper _licenseeCurrencyCodeMapper;
        private readonly ILicenseeLanguageCodeMapper _licenseeLanguageCodeMapper;
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly ILocalizationOperations _localizationOperations;

        public GameProviderController(
            ILicenseeOperations licenseeOperations,
            ILocalizationOperations localizationOperations,
            ILicenseeLanguageCodeMapper licenseeLanguageCodeMapper,
            ILicenseeCurrencyCodeMapper licenseeCurrencyCodeMapper)
        {
            _licenseeOperations = licenseeOperations;
            _localizationOperations = localizationOperations;
            _licenseeLanguageCodeMapper = licenseeLanguageCodeMapper;
            _licenseeCurrencyCodeMapper = licenseeCurrencyCodeMapper;
        }

        /// <summary>
        /// Get game provider list
        /// </summary>
        [HttpPost, Route("list")]
        public async Task<GetGameProvidersResponse> GetGameProvidersAsync(GetGameProvidersRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            var currencyCode =
                await _licenseeCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, licenseeId);
            var gameProviders = await _licenseeOperations.GetLicenseeGameProvidersAsync(licenseeId, currencyCode,
                request.PlatformType, request.GameTypes);

            var culture = await _licenseeLanguageCodeMapper.ExternalToInternalAsync(request.Language, licenseeId);

            var statuses = request.IncludeMaintenance ? ValidStatus.Default : ValidStatus.ClosingBet;

            var providers = await Task.WhenAll(gameProviders
                .Where(gp => statuses.Contains(gp.Status))
                .Select(async gp =>
                {
                    var key = gp.Id.ToString();
                    var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gp.Id, culture);
                    return new GetGameProvidersResponse.GameProviderModel
                    {
                        Id = gp.Id,
                        Code = gp.Code,
                        Name = _localizationOperations.Parse(store, key, GameProviderName, gp.Name),
                        Description = _localizationOperations.Parse(store, key, GameProviderDescription, gp.Description),
                        Status = gp.Status
                    };
                }));

            return new GetGameProvidersResponse
            {
                GameProviders = providers
            };
        }
    }
}