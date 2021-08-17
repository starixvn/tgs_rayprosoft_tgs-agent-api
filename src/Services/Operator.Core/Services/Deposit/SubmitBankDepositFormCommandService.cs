using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Utility;
using Tgs.FileService.Clients;
using Tgs.Payment.Clients;
using TGS.Payment.ManualDeposit;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class SubmitBankDepositFormCommandService : ICommandService<SubmitBankDepositFormCommand>
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly ManualDepositSerializer _serializer;

        public SubmitBankDepositFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IFileServiceClient fileServiceClient,
            ManualDepositSerializer serializer)
        {
            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _fileServiceClient = fileServiceClient ?? throw new ArgumentNullException(nameof(fileServiceClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async ValueTask ExecuteAsync(SubmitBankDepositFormCommand command,
            CancellationToken cancellationToken = default)
        {
            var randomFileName = StringGenerator.GenerateAlphanumericString(32);

            var uploadResult = await _fileServiceClient
                .UploadFileAsync(
                    $"/{randomFileName}",
                    MediaTypeHeaderValue.Parse(command.ContentType),
                    command.FileStream)
                .ConfigureAwait(false);

            await _paymentServiceClient
                .SubmitDepositFormStatusAsync(
                    command.FormId,
                    PaymentFlowState.WaitApprove,
                    _serializer.Serialize(
                        new ManualDepositPaymentReturn(uploadResult.Id)))
                .ConfigureAwait(false);
        }
    }
}