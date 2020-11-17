using FileService.Resources;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace FileService.Controllers.Messages.Validators
{
    public class FileValidator : AbstractValidator<UploadRequest>
    {
        private readonly IOptions<FileSetting> _options;

        public FileValidator(IOptions<FileSetting> options)
        {
            _options = options;

            RuleFor(x => x.File.Length)
                .ExclusiveBetween(0, _options.Value.FileSizeLimit)
                .WithMessage(x => File.FileMaxLength);

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage(File.ImageNotFound)
                .NotImage()
                .WithMessage(File.InvalidImageSignature);
        }
    }
}