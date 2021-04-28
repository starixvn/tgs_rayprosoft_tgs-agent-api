using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TGS.Apis.FileService.Clients;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Payment.ManualDeposit;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Deposit;

namespace TGS.Services.Operator.Deposit
{
    internal class SubmitBankDepositFormCommandService : ICommandService<SubmitBankDepositFormCommand>
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ManualDepositSerializer _serializer;

        public SubmitBankDepositFormCommandService(
            IFileServiceClient fileServiceClient,
            IPaymentRepository paymentRepository,
            ManualDepositSerializer serializer)
        {
            _fileServiceClient = fileServiceClient ?? throw new ArgumentNullException(nameof(fileServiceClient));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
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

            await _paymentRepository
                .SubmitFormStatusAsync(
                    command.FormId,
                    1000,
                    _serializer.Serialize(
                        new ManualDepositPaymentReturn(uploadResult.Id)))
                .ConfigureAwait(false);
        }
    }
}
