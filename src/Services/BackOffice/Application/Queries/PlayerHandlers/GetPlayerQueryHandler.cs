using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayerQueryHandler : Handler<GetPlayerRequest, GetPlayerResponse>
    {
        private readonly IPlayerService _service;

        public GetPlayerQueryHandler(IPlayerService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<GetPlayerResponse> Handle(GetPlayerRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _service.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode);

            if (player == null)
            {
                return CreateInvalidResponse<GetPlayerResponse>(Player.NotFound);
            }

            return new()
            {
                Valid = true,
                Player = new()
                {
                    Username = player.Username,
                    RealName = player.RealName,
                    VipLevel = player.Vip?.Level,
                    Status = player.Status,
                    StatusName = player.Status.ToString(),
                    HasAgent = player.HasAgent,
                    RegisteredOn = player.RegisteredOn,
                    ParentUsername = player.ParentPlayer?.Username,
                    Balance =
                        player.Wallets.FirstOrDefault() == null ? 0 : player.Wallets.FirstOrDefault().Balance,
                    Phone = player.Phone,
                    Email = player.Email,
                    QQ = player.QQ,
                    WeChat = player.Wechat,
                    Banks = player.Banks.Select(p => new PlayerModel.BankModel
                    {
                        Id = p.Id, RealName = p.RealName, BankName = p.BankName, BankAccount = p.BankAccount
                    }).ToArray()
                }
            };
        }
    }
}