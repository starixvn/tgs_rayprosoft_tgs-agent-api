using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerBank;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerBankHandlers
{
    public class DeletePlayerBankHandler : Handler<DeletePlayerBankRequest, MessageResponse>
    {
        private readonly IPlayerBankService _service;

        public DeletePlayerBankHandler(IPlayerBankService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(DeletePlayerBankRequest request,
            CancellationToken cancellationToken)
        {
            var bank = await _service.GetPlayerBankAsync(x => x.PlayerCode == request.PlayerCode && x.Id == request.Id);

            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.BankNotFound);
            }

            await _service.DeletePlayerBankAsync(bank);

            return CreateValidResponse<MessageResponse>();
        }
    }
}