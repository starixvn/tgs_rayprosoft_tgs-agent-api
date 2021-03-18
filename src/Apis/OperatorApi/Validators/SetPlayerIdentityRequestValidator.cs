using FluentValidation;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.IdentityVerifiers;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Validators
{
    public class SetPlayerIdentityRequestValidator : BaseFluentValidator<SetPlayerIdentityRequest>
    {
        public SetPlayerIdentityRequestValidator(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IIdentityVerifierSelector<string> identityVerifierSelector)
        {
            RuleFor(x => x.RealName).NotNull().NotEmpty();

            RuleFor(x => x.Identity).MustAsync(async (id, _) =>
            {
                var httpContext = httpContextAccessor.HttpContext;
                var playerTokenData = httpContext.Items.GetPlayerTokenData();

                var player = await playerOperations
                    .GetPlayerByCodeAsync(playerTokenData.PlayerCode)
                    .ConfigureAwait(false);

                var playerCurrency = player.DefaultWalletCurrencyCode;

                var identityVerifier = identityVerifierSelector.GetIdentityVerifier(playerCurrency);

                return identityVerifier.Verify(id);
            });
        }
    }
}
