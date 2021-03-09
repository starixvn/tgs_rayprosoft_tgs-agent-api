using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator.Properties;
using Valhalla.Services;

namespace TGS.Services.Operator
{
    internal class ForgotPasswordCommandService : ICommandService<ForgotPasswordCommand>
    {
        private readonly ITgsRepository _tgsRepository;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ForgotPasswordCommandService(
            ITgsRepository tgsRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(ForgotPasswordCommand command)
        {
            var player = _tgsRepository.Players
                .Where(player => player.PhoneAuthentication == true && player.Phone == command.Phone)
                .FirstOrDefault();

            if (player == null)
                throw new PlayerNotFoundException(command.Phone);

            var validateCode = _codeGenerator.GenerateCode();
            var token = _codeGenerator.GenerateToken();

            var data = new ValidateCodeData(player.PlayerCode, token);

            await _validateCodeOperator
                .SaveValidateDataAsync(validateCode, data)
                .ConfigureAwait(false);

            var shortMessage = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ForgotPasswordMessage), command.CultureInfo),
                validateCode);

            await _validateCodeOperator
                .SendMessageAsync(player.Phone, shortMessage)
                .ConfigureAwait(false);
        }
    }
}
