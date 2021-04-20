using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TGS.Apis.FileService.Clients;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class SubmitBankDepositFormCommandService : ICommandService<SubmitBankDepositFormCommand>
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public SubmitBankDepositFormCommandService(
            IFileServiceClient fileServiceClient,
            IPaymentRepository paymentRepository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _fileServiceClient = fileServiceClient ?? throw new ArgumentNullException(nameof(fileServiceClient));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
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
                    _jsonSerializationProvider.SerializeToString(
                        new
                        {
                            ManualReceiptFileId = uploadResult.Id
                        }))
                .ConfigureAwait(false);
        }
    }
}
