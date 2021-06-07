using FluentValidation;
using TGS.Services.Report.Application.Models.Player;

namespace TGS.Apis.ReportApi.Validations.Player
{
    public class GetPlayerAccountChangesRequestValidator : AbstractValidator<GetPlayerAccountChangesRequest>
    {
        public GetPlayerAccountChangesRequestValidator()
        {
            RuleFor(p => p.PlayerCode)
                .NotEmpty()
                .WithMessage(Resources.Player.PlayerCodeRequired);
        }
    }
}