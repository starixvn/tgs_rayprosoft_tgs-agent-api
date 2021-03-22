using Microsoft.Extensions.Options;
using TGS.Services.Operator;

namespace TGS.Apis.OperatorApi
{
    public class ConstValidateCodeGenerator : IValidateCodeGenerator
    {
        private readonly IValidateCodeGenerator _validateCodeGenerator;
        private readonly ConstValidateCodeOptions _options;

        public ConstValidateCodeGenerator(IValidateCodeGenerator validateCodeGenerator, IOptions<ConstValidateCodeOptions> optionsAccessor)
        {
            _validateCodeGenerator = validateCodeGenerator;
            _options = optionsAccessor.Value;
        }

        public string GenerateCode()
        {
            if (_options.ValidateCodeTestEnabled)
                return "123456";
            else
                return _validateCodeGenerator.GenerateCode();
        }

        public string GenerateSecurityKey() => _validateCodeGenerator.GenerateSecurityKey();
    }
}
