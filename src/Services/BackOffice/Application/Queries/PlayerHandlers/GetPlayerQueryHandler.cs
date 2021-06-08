using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayerQueryHandler : Handler<GetPlayerRequest, GetPlayerResponse>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _repository;
        private readonly IPlayerService _service;

        public GetPlayerQueryHandler(IPlayerService service, ITgsRepository repository, IHttpContextAccessor accessor,
            IJsonSerializationProvider jsonSerializationProvider) :
            base(accessor)
        {
            _service = service;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public override async Task<GetPlayerResponse> Handle(GetPlayerRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _service.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode);

            if (player == null)
            {
                return CreateInvalidResponse<GetPlayerResponse>(Player.NotFound);
            }

            var playerBuckets = await _repository.PlayerBuckets.AsQueryable()
                .Where(x => x.PlayerCode == player.PlayerCode).ToArrayAsync(cancellationToken);

            return new()
            {
                Valid = true,
                Player = new()
                {
                    Username = player.Username,
                    RealName = player.RealName,
                    VipLevel = player.Vip?.Level,
                    VipId = player.Vip?.Id,
                    Status = player.Status,
                    StatusName = player.Status.ToString(),
                    RegisteredOn = player.RegisteredOn,
                    AgentUsername = player.Agent?.Username,
                    Balance =
                        player.Wallets.FirstOrDefault() == null ? 0 : player.Wallets.FirstOrDefault().Balance,
                    Phone = player.Phone,
                    Email = player.Email,
                    QQ = player.QQ,
                    WeChat = player.Wechat,
                    Banks = player.Banks.Select(p => new PlayerModel.BankModel
                    {
                        Id = p.Id,
                        PaymentChannelId = p.PaymentChannelId,
                        BankType = p.BankType,
                        Detail = _jsonSerializationProvider.DeserializeFromString<JsonDocument>(p.Detail)
                    }).ToArray(),
                    WageringRequirement = playerBuckets.Sum(p => p.BucketAmount) -
                        playerBuckets.Sum(p => p.CurrentBucketAmount)
                }
            };
        }
    }
}