using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TGS.Apis.FileService.Clients;
using TGS.Core.Infrastructures.Utility;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class SubmitBankDepositFormCommandService : ICommandService<SubmitBankDepositFormCommand>
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IWalletRepository _walletRepository;

        public SubmitBankDepositFormCommandService(
            IFileServiceClient fileServiceClient,
            IWalletRepository walletRepository)
        {
            _fileServiceClient = fileServiceClient ?? throw new ArgumentNullException(nameof(fileServiceClient));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask ExecuteAsync(SubmitBankDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var randomFileName = StringGenerator.GenerateAlphanumericString(32);

            var uploadResult = await _fileServiceClient
                .UploadFileAsync(
                    $"/{randomFileName}",
                    MediaTypeHeaderValue.Parse(command.ContentType),
                    command.FileStream)
                .ConfigureAwait(false);

            await _walletRepository
                .SubmitRemittanceReceiptToDepositFormAsync(command.FormId, uploadResult.Id)
                .ConfigureAwait(false);
        }
    }
}
