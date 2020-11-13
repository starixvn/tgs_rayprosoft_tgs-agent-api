using System.IO;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
            int minimumLength = 8)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(Resources.User.PasswordRequired)
                .MinimumLength(minimumLength).WithMessage(Resources.User.PasswordLengthAtLeast)
                .Matches("[0-9]").WithMessage(Resources.User.PasswordNeedNumber)
                .Matches("[^a-zA-Z0-9]").WithMessage(Resources.User.PasswordNeedSpecialCharacter);
            return options;
        }

        public static IRuleBuilderOptions<T, IFormFile> NotImage<T>(
            this IRuleBuilder<T, IFormFile> ruleBuilder)
        {
            return ruleBuilder.Must(p =>
            {
                var ext = Path.GetExtension(p.FileName).ToLowerInvariant();
                using var reader = new BinaryReader(p.OpenReadStream());
                var signatures = ImageFileSignature.FileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            });
        }
    }
}