using FluentValidation;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.IdentityVerifiers;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Validators;

public class SetPlayerIdentityRequestValidator : BaseFluentValidator<SetPlayerIdentityRequest>
{
    public SetPlayerIdentityRequestValidator(
        IHttpContextAccessor httpContextAccessor,
        IPlayerOperations playerOperations,
        IIdentityVerifierSelector<string> identityVerifierSelector)
    {
        RuleFor(x => x.RealName)
            .NotNull()
            .NotEmpty()
            .WithName(x => GetJsonPropertyName(x, nameof(x.RealName)));

        RuleFor(x => x.Identity)
            .Must(id =>
            {
                var httpContext = httpContextAccessor.HttpContext;
                var playerTokenData = httpContext.Items.GetPlayerTokenData();

                var player = playerOperations
                    .GetPlayerByCodeAsync(playerTokenData.PlayerCode)
                    .GetAwaiter()
                    .GetResult();

                var playerCurrency = player.DefaultWalletCurrencyCode;

                var identityVerifier = identityVerifierSelector.GetIdentityVerifier(playerCurrency);

                return identityVerifier.Verify(id);
            })
            .WithName(x => GetJsonPropertyName(x, nameof(x.Identity)));
    }
}
