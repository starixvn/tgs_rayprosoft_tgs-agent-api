using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerBank;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerBankHandlers
{
    public class UpdatePlayerBankHandler : Handler<UpdatePlayerBankRequest, MessageResponse>
    {
        private readonly IPlayerBankService _service;

        public UpdatePlayerBankHandler(IPlayerBankService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdatePlayerBankRequest request,
            CancellationToken cancellationToken)
        {
            var bank = await _service.GetPlayerBankAsync(q => q.PlayerCode == request.PlayerCode && q.Id == request.Id);

            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.BankNotFound);
            }

            await _service.UpdatePlayerBankAsync(bank, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}