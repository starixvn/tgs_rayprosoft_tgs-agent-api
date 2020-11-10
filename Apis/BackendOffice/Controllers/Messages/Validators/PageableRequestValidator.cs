using FluentValidation;
using TGS.Apis.BackendOffice.Resources;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators
{
    public class PageableRequestValidator : AbstractValidator<PageableRequest>
    {
        public PageableRequestValidator()
        {
            RuleFor(x => x.Rows)
                .GreaterThan(0)
                .WithMessage(Paging.RowsGreaterZero);

            RuleFor(x => x.SortBy)
                .NotNull()
                .WithMessage(Paging.SortByRequired);
        }
    }
}