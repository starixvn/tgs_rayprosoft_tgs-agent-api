using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;
using Tgs.ProfitRebate.Client;
using Tgs.ProfitRebate.Client.Models;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs.Agents
{
    public interface IAgentDividendWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null);
    }

    public class AgentDividendWorkerService : IAgentDividendWorkerService
    {
        private readonly IBrandGameProviderService _brandService;
        private readonly IProfitRebateClient _client;

        public AgentDividendWorkerService(
            IProfitRebateClient client, IBrandGameProviderService brandService)
        {
            _client = client;
            _brandService = brandService;
        }

        async Task<TgsResponse> IAgentDividendWorkerService.ExecutionAsync()
        {
            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));
            var queryDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, new(8, 0, 0));
            var brands = await _brandService.TgsRepository.Brands.AsQueryable()
                .Where(x => queryDate.ToUniversalTime() > x.CurrentSettlementTo)
                .Select(x => new { x.Code })
                .ToArrayAsync();

            var responses = new List<TgsResponse>();

            foreach (var brand in brands)
            {
                var response = await _client.CreateAgentDividendAsync(new(brand.Code));
                if (response.Message != null)
                {
                    responses.Add(new TgsResponse { ErrorMessage = $"{brand} : {response.Message}" });
                }
            }

            return new() { ErrorMessage = responses.Count > 0 ? responses.ToString() : "" };
        }

        async Task<TgsResponse> IAgentDividendWorkerService.ManualExecutionAsync(string brandCode)
        {
            var response =
                await _client.CreateAgentDividendAsync(new CreateAgentDividendsRequest(brandCode));
            return new() { ErrorMessage = response?.Message };
        }
    }
}