using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.Agent.Exceptions;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    internal class ForgotPasswordCommandService : IGetService<ForgotPasswordCommand, string>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGetService<ForgotPasswordByEmailCommand, string> _forgotByMailService;
        private readonly IGetService<ForgotPasswordByPhoneCommand, string> _forgotByPhoneService;

        public ForgotPasswordCommandService(
            IBrandRepository brandRepository,
            IGetService<ForgotPasswordByEmailCommand, string> forgotByMailService,
            IGetService<ForgotPasswordByPhoneCommand, string> forgotByPhoneService)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _forgotByMailService = forgotByMailService ?? throw new ArgumentNullException(nameof(forgotByMailService));
            _forgotByPhoneService = forgotByPhoneService ?? throw new ArgumentNullException(nameof(forgotByPhoneService));
        }

        public async ValueTask<string> GetAsync(ForgotPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false) ?? throw new BrandNotFoundException();

            if (brand.AgentSmsEnabled)
            {
                var forgotPasswordByPhoneCommand = new ForgotPasswordByPhoneCommand(
                    command.BrandId,
                    command.CultureInfo,
                    command.EmailOrPhone);

                return await _forgotByPhoneService
                    .GetAsync(forgotPasswordByPhoneCommand, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var forgetPasswordByMailCommand = new ForgotPasswordByEmailCommand(
                    command.BrandId,
                    command.CultureInfo,
                    command.EmailOrPhone);

                return await _forgotByMailService
                    .GetAsync(forgetPasswordByMailCommand, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
