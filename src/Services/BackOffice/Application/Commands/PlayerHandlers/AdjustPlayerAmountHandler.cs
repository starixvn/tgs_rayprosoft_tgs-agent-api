using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Tgs.CashService.Client;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Logging;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class AdjustPlayerAmountHandler : Handler<CreatePlayerAdjustmentRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;
        private readonly ITgsLog _tgsLog;

        public AdjustPlayerAmountHandler(
            IAgentService agentService,
            IAgentWalletRepository agentWalletRepository,
            ICashServiceClient cashServiceClient,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor,
            ITgsLog tgsLog) : base(accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));

            _agentWalletRepository =
                agentWalletRepository ?? throw new ArgumentNullException(nameof(agentWalletRepository));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));

            _playerBucketRepository =
                playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));

            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public override async Task<MessageResponse> Handle(
            CreatePlayerAdjustmentRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode);

            if (player is null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.NotFound);
            }

            var isTransferToAgent = IsTransferToAgent(request);

            var targetAgent = isTransferToAgent
                ? await _agentService
                    .GetAgentAsync(x => x.Id == player.AgentId)
                    .ConfigureAwait(false)
                : null;

            if (isTransferToAgent && targetAgent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.TargetAgentNotFound);
            }

            var wallet = player.Wallets.FirstOrDefault(x => x.Id == player.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (request.AdjustType == AdjustmentType.Subtract && wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            using var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var remark = _jsonSerializationProvider.SerializeToString(new
            {
                request.TransactionType,
                TargetAgent = targetAgent?.Username,
                request.Remark
            });

            if (request.Amount != 0M)
            {
                await _cashServiceClient
                    .AdjustAsync(
                        player.DefaultWalletId,
                        request.AdjustType == AdjustmentType.Subtract ? -request.Amount : request.Amount,
                        remark,
                        null,
                        Guid.Parse(UserId))
                    .ConfigureAwait(false);

                if (isTransferToAgent)
                {
                    await _agentWalletRepository.ExchangeAsync(
                        targetAgent.BrandId,
                        targetAgent.DefaultWalletId,
                        targetAgent.Id,
                        TransactionTypes.Adjust,
                        request.Amount).ConfigureAwait(false);
                }
            }

            if (request.BucketAmount > 0)
            {
                await _playerBucketRepository
                    .CreatePlayerBucketsAsync(new[]
                    {
                        new CreatePlayerBucketRequest(
                            player.BrandId,
                            player.PlayerCode,
                            player.DefaultWalletId,
                            BucketType.Adjustment,
                            request.BucketAmount)
                    })
                    .ConfigureAwait(false);

                _tgsLog.LogInfo($"Adjust add bucket {player.PlayerCode}: {request.BucketAmount}");
            }

            tranScope.Complete();

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTransferToAgent(CreatePlayerAdjustmentRequest request) =>
            request.AdjustType == AdjustmentType.Subtract &&
            request.TransactionType == PlayerAdjustmentTransactionType.TransferAdjust;
    }
}