using Microsoft.Extensions.Options;
using TGS.Services.Operator;

namespace TGS.Apis.AgentApi
{
    public class ConstValidateCodeGenerator : IValidateCodeGenerator
    {
        private readonly ConstValidateCodeOptions _options;
        private readonly IValidateCodeGenerator _validateCodeGenerator;

        public ConstValidateCodeGenerator(IValidateCodeGenerator validateCodeGenerator,
            IOptions<ConstValidateCodeOptions> optionsAccessor)
        {
            _validateCodeGenerator = validateCodeGenerator;
            _options = optionsAccessor.Value;
        }

        public string GenerateCode()
        {
            if (_options.ValidateCodeTestEnabled)
            {
                return "123456";
            }

            return _validateCodeGenerator.GenerateCode();
        }

        public string GenerateSecurityKey() => _validateCodeGenerator.GenerateSecurityKey();
    }
}