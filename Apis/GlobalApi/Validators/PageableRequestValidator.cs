using FluentValidation;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.GlobalApi.Validators
{
    public abstract class PageableRequestValidator<T> : BaseFluentValidator<T> where T : PageableRequest
    {
        protected PageableRequestValidator()
        {
            RuleFor(x => x.ShowAll)
                .NotNull()
                .WithName(x => GetBindPropertyName(x, nameof(x.ShowAll)));

            RuleFor(x => x.Rows)
                .GreaterThan(0)
                .When(x => x.ShowAll == false)
                .WithName(x => GetBindPropertyName(x, nameof(x.Rows)));

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .When(x => x.ShowAll == false)
                .WithName(x => GetBindPropertyName(x, nameof(x.Rows)));
        }
    }
}
