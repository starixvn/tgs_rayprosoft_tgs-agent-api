using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.DataServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;

namespace TGS.Apis.GlobalApi.Validators
{
    public sealed class GetBrandPlayersRequestValidator : PageableRequestValidator<GetBrandPlayersRequest>
    {
        public GetBrandPlayersRequestValidator(ITgsRepository repository, ISettingOperations settingOperations,
            IWebHostEnvironment env)
        {
            var isProduction = env.IsProduction();
            var environment = env.EnvironmentName;

            RuleFor(x => x.BrandCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.BrandCode)));

            RuleFor(x => x.BrandCode)
                .MustAsync((brandCode, _) => ValidateBrandCodeAsync(brandCode, repository, settingOperations))
                .When(_ => isProduction)
                .WithMessage(x =>
                    $"The brand {GetBindPropertyName(x, nameof(x.BrandCode))} value is not allowed on '{environment}'");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.PlayerType)
                .Must(type => ValidatePlayerType(type, isProduction))
                .WithMessage(x =>
                    $"The {GetBindPropertyName(x, nameof(x.PlayerType))} value is not allowed on '{environment}'");
        }

        private static bool ValidatePlayerType(PlayerType type, bool isProduction)
        {
            if (type == PlayerType.None)
            {
                return false;
            }

            var assumeType = isProduction ? PlayerType.TestPlayer : PlayerType.All;
            return (assumeType & type) == type;
        }

        private static async Task<bool> ValidateBrandCodeAsync(string brandCode, ITgsRepository repository,
            ISettingOperations settingOperations)
        {
            var licenseeCodes = await settingOperations.GetValueFromJsonByKeyAsync<string[]>(SettingConsts.SuperLicenseeCodes);
            return await repository.Brands.AnyAsync(b =>
                b.Code == brandCode && licenseeCodes.Contains(b.Licensee.Code));
        }
    }
}