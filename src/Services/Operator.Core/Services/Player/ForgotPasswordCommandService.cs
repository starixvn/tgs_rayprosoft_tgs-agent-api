using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;
using TGS.SmsService.Client;

namespace TGS.Services.Operator.Services.Player
{
    internal class ForgotPasswordCommandService : IGetService<ForgotPasswordCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly ISysRepository _sysRepository;
        private readonly ITgsRepository _tgsRepository;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ForgotPasswordCommandService(
            ITgsRepository tgsRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            ISysRepository sysRepository,
            ISmsServiceClient smsServiceClient)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
        }

        public async ValueTask<string> GetAsync(ForgotPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var player = _tgsRepository.Players.AsQueryable()
                .Where(player => player.Email == command.Email)
                .FirstOrDefault();

            if (player == null)
                throw new PlayerNotFoundException($"The email ({command.Email}) can't find the player info.");

            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string Token)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new ValidateCodeData(player.PlayerCode, token);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var brandInfo = await _sysRepository
                .GetBrandInfoAsync(player.BrandId)
                .ConfigureAwait(false);

            if (brandInfo!.SmsEnabled)
            {
                if (!player.PhoneAuthentication)
                    throw new PlayerPhoneNotSetException();

                await _smsServiceClient.SendShortMessageAsync(
                    $"{player.AreaCode}{player.Phone}",
                    saveResult.ValidateCode,
                    player.BrandId).ConfigureAwait(false);
            }
            else
                await _validateCodeOperator.SendEmailAsync(
                    command.CultureInfo,
                    player.Email,
                    saveResult.ValidateCode).ConfigureAwait(false);

            return saveResult.Token;
        }
    }
}
