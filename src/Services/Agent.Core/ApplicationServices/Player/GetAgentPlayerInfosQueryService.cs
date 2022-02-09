using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Agent.ApplicationServices.Player
{
    internal class GetAgentPlayerInfosQueryService : IQueryService<GetAgentPlayerInfosQuery, AgentPlayerInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetAgentPlayerInfosQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<AgentPlayerInfo> QueryAsync(
            GetAgentPlayerInfosQuery query,
            CancellationToken cancellationToken = default)
        {
            var playerQuery = _tgsRepository.Players
                .AsQueryable()
                .Where(player => player.AgentId == query.AgentId);

            if (!string.IsNullOrWhiteSpace(query.Username))
                playerQuery = playerQuery.Where(player => player.Username == query.Username);

            if (query.Status != null)
                playerQuery = playerQuery.Where(player => player.Status == query.Status);

            if (query.VipId != null)
                playerQuery = playerQuery.Where(player => player.VipId == query.VipId);

            var playerIpQuery = _tgsRepository.PlayerIpRecords
                .AsQueryable()
                .Where(r => r.IpType == 1);

            var players =
                from player in playerQuery
                join wallet in _tgsRepository.Wallets
                   on player.DefaultWalletId equals wallet.Id
                join vip in _tgsRepository.VipLevels
                    on player.VipId equals vip.Id into vipGroup
                from vip in vipGroup.DefaultIfEmpty()
                join ipRecord in playerIpQuery
                    on player.PlayerCode equals ipRecord.PlayerCode into ipRecordGroup
                from ipRecord in ipRecordGroup.DefaultIfEmpty()
                select new AgentPlayerInfo(
                    player.PlayerCode,
                    player.Username,
                    wallet.Balance,
                    ipRecord.RecordOn,
                    player.AreaCode,
                    player.Phone,
                    ipRecord.Host,
                    player.Status,
                    vip.Level);

            if (!string.IsNullOrWhiteSpace(query.RegisterHost))
            {
                players = players.Where(p =>
                    p.RegisterHost != null &&
                    p.RegisterHost.ToLower() == query.RegisterHost.ToLower());
            }

            if (query.BeginRegisteredOn != null && query.EndRegisteredOn != null)
            {
                players = players.Where(p =>
                    p.RegisteredTime > query.BeginRegisteredOn &&
                    p.RegisteredTime <= query.EndRegisteredOn);
            }

            return players.ToAsyncEnumerable();
        }
    }
}
