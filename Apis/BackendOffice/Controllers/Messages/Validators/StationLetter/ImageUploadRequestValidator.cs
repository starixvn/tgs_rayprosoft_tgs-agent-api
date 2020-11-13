using FluentValidation;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Controllers.Messages.StationLetter;
using TGS.Core.Infrastructures.File;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.StationLetter
{
    public class ImageUploadRequestValidator : AbstractValidator<ImageUploadRequest>
    {
        private readonly IOptions<FileSetting> _options;

        public ImageUploadRequestValidator(IOptions<FileSetting> options)
        {
            _options = options;

            RuleFor(x => x.FormFile.Length)
                .ExclusiveBetween(0, _options.Value.FileSizeLimit)
                .WithMessage(x => Resources.StationLetter.FileMaxLength);

            RuleFor(x => x.FormFile)
                .NotImage();
        }
    }
}