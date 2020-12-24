using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class UpdatePlayerRequestValidator : BaseFluentValidator<UpdatePlayerRequest>
    {
        public UpdatePlayerRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => x.Email != null)
                .WithName(x => GetJsonPropertyName(x, nameof(x.Email)));
        }
    }
}
