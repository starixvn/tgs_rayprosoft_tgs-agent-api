using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.PlayerBucket;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class ClearPlayerBucketsWithDepositHandler : Handler<ClearPlayerBucketsWithDepositRequest, MessageResponse>
    {
        private readonly IMediator _mediator;
        private readonly IPlayerBucketRepository _playerBucketRepository;

        public ClearPlayerBucketsWithDepositHandler(IMediator mediator,
            IPlayerBucketRepository playerBucketRepository,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
            _playerBucketRepository = playerBucketRepository;
        }


        public override async Task<MessageResponse> Handle(ClearPlayerBucketsWithDepositRequest request,
            CancellationToken cancellationToken)
        {
            var playerAllBalances = await _mediator.Send(new GetPlayerGameProviderBalanceRequest(request.PlayerCode),
                cancellationToken);

            var balance = playerAllBalances.Sum(x => x.Balance) ?? 0M;

            if (balance <= 1M)
            {
                await _playerBucketRepository.ClearPlayerBucketsAsync(request.PlayerCode);
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}