using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerBank;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerBankHandlers
{
    public class CreatePlayerBankHandler : Handler<CreatePlayerBankRequest, MessageResponse>
    {
        private readonly IPlayerBankService _service;

        public CreatePlayerBankHandler(IPlayerBankService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreatePlayerBankRequest request,
            CancellationToken cancellationToken)
        {
            await _service.CreatePlayerBankAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}