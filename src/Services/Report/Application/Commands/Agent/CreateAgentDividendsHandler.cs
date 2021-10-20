using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs.Models;
using Tgs.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Services.Report.Application.Commands.Agent
{
    public class
        CreateAgentDividendsHandler : Handler<CreateAgentDividendsRequest,
            MessageResponse>
    {
        private readonly IAgentDividendService _agentDividendService;
        private readonly ITgsAgentService _agentService;
        private readonly ITgsBrandService _brandService;
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly IPlayerGameAggregateService _playerGameAggregateService;
        private readonly IAgentWalletRepository _walletRepository;

        public CreateAgentDividendsHandler(
            IPlayerAggregateService playerAggregateService,
            IPlayerGameAggregateService playerGameAggregateService,
            ITgsAgentService agentService,
            ITgsBrandService brandService,
            IAgentDividendService agentDividendService,
            IAgentWalletRepository walletRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _brandService = brandService;
            _playerAggregateService = playerAggregateService;
            _playerGameAggregateService = playerGameAggregateService;
            _agentDividendService = agentDividendService;
            _walletRepository = walletRepository;
        }

        public override async Task<MessageResponse> Handle(
            CreateAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));
            var queryDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, new(8, 0, 0));

            await SendAgentDividend(request.BrandCode, queryDate, cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }

        private static (DateTimeOffset, DateTimeOffset) GetInterval(
            SettlementIntervalType type, DateTimeOffset startDate, int? dayOfMonth) => type switch
        {
            SettlementIntervalType.Daily => (startDate, startDate),
            SettlementIntervalType.Weekly => (DateGenerator.GetFirstDayOfWeek(startDate),
                DateGenerator.GetLastDayOfWeek(startDate)),
            SettlementIntervalType.Monthly => (startDate,
                dayOfMonth == null
                    ? DateGenerator.GetLastDayOfMonth(startDate.AddMonths(1))
                    : DateGenerator.GetSpecifyDayOfMonth(startDate.AddMonths(1), dayOfMonth.Value!)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        private async IAsyncEnumerable<Brand[]> GetBrands(
            string brandCode, DateTimeOffset queryDate,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            bool hasBrands;

            do
            {
                var brands = await _brandService.GetBrandQueries(q
                            =>
                            q.Where(brandCode is not null, x => x.Code == brandCode)
                                .Where(brandCode is null, x => queryDate > x.CurrentSettlementTo),
                        true)
                    .ToArrayAsync(cancellationToken);
                hasBrands = brands.Any();

                yield return brands;
            } while (hasBrands);
        }

        private async Task SendAgentDividend(
            string brandCode, DateTimeOffset queryDate,
            CancellationToken cancellationToken)
        {
            await foreach (var brands in GetBrands(brandCode, queryDate, cancellationToken))
            {
                var createModels = new List<CreateAgentDividendModel>();
                var createAutoSendModels = new List<CreateAgentDividendModel>();
                var updateAgentDividends = new List<AgentDividend>();

                var maxNumber = await _agentDividendService.GetAgentDividendQueries()
                    .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

                foreach (var brand in brands)
                {
                    if (!await _agentDividendService.GetAgentDividendQueries()
                        .AnyAsync(
                            x => x.BrandId == brand.Id && brand.CurrentSettlementFrom == x.From &&
                                brand.CurrentSettlementTo == x.To,
                            cancellationToken))
                    {
                        var (autoSendDividends, manuelOrNegativeDividends, agentDividends) =
                            await _agentDividendService.GetAgentDividends(brand, maxNumber);

                        maxNumber += autoSendDividends.Count() + manuelOrNegativeDividends.Count();

                        createAutoSendModels.AddRange(autoSendDividends);
                        createModels.AddRange(manuelOrNegativeDividends);
                        updateAgentDividends.AddRange(agentDividends);
                    }

                    var (nextFrom, nextTo) =
                        GetInterval(brand.SettlementIntervalType,
                            brand.NextSettlementTo.AddDays(1),
                            brand.SettlementDay);

                    brand.PreviousSettlementFrom = brand.CurrentSettlementFrom;
                    brand.PreviousSettlementTo = brand.CurrentSettlementTo;
                    brand.CurrentSettlementFrom = brand.NextSettlementFrom;
                    brand.CurrentSettlementTo = brand.NextSettlementTo;
                    brand.NextSettlementFrom = nextFrom;
                    brand.NextSettlementTo = nextTo;
                }

                await SaveAgentDividendData(brands,
                    createModels,
                    createAutoSendModels,
                    updateAgentDividends,
                    cancellationToken);
            }
        }

        private async Task SaveAgentDividendData(
            Brand[] brands, List<CreateAgentDividendModel> createModels,
            List<CreateAgentDividendModel> createAutoSendModels, List<AgentDividend> updateAgentDividends,
            CancellationToken cancellationToken)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _agentDividendService.CreateAgentDividendsAsync(createModels.ToArray(), null);
            await _agentDividendService.UpdateAgentDividendsAsync(updateAgentDividends.ToArray());

            await _agentDividendService.CreateAgentDividendsAsync(
                createAutoSendModels.ToArray(),
                DateTimeOffset.Now);

            await _brandService.UpdateBrandsAsync(brands);

            var createdAgentIds = createAutoSendModels.Select(x => x.AgentId).ToArray();

            var createdAgents = await _agentService.GetAgentQueries().Where(x => createdAgentIds.Contains(x.Id))
                .ToArrayAsync(cancellationToken);

            foreach (var models in createAutoSendModels)
            {
                var agent = createdAgents.Single(a => a.Id == models.AgentId);

                await _walletRepository.ExchangeAsync(models.BrandId,
                    agent.DefaultWalletId,
                    agent.Id,
                    TransactionTypes.Dividend,
                    models.AgentDividendAmount);
            }

            scope.Complete();
        }
    }
}