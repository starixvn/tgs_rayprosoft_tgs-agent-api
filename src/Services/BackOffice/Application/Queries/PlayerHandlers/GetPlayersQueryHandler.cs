using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayersQueryHandler : Handler<GetPlayersRequest, PagedResponse<PlayersModel>>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetPlayersQueryHandler(
            ITgsRepository tgsRepository,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _tgsRepository = tgsRepository ?? throw new System.ArgumentNullException(nameof(tgsRepository));
        }

        public override Task<PagedResponse<PlayersModel>> Handle(
            GetPlayersRequest request,
            CancellationToken cancellationToken)
        {
            var queryableResellers = _tgsRepository.Resellers
                .Include(r => r.Licensees)
                .ThenInclude(l => l.Brands)
                .AsQueryable();

            if (ResellerId != null)
                queryableResellers = queryableResellers.Where(reseller => reseller.Id == ResellerId);

            var queryableLicensees = queryableResellers.SelectMany(reseller => reseller.Licensees);

            if (LicenseeId != null)
                queryableLicensees = queryableLicensees.Where(licensee => licensee.Id == LicenseeId);

            var queryableBrands = queryableLicensees.SelectMany(licensee => licensee.Brands);

            if (BrandId != null)
                queryableBrands = queryableBrands.Where(brand => brand.Id == BrandId);

            if (request.BrandId != null)
                queryableBrands = queryableBrands.Where(brand => brand.Id == request.BrandId);

            var playerQuery = _tgsRepository.Players
                .Include(player => player.Vip)
                .Include(player => player.Agent)
                .Include(player => player.Wallets)
                .AsQueryable();

            if (request.BrandId != null)
                playerQuery = playerQuery.Where(player => player.BrandId == request.BrandId);

            if (!string.IsNullOrWhiteSpace(request.Username))
                playerQuery = playerQuery.Where(player => player.Username == request.Username);

            if (request.Status != null)
                playerQuery = playerQuery.Where(player => player.Status == request.Status);

            if (request.VipId != null)
                playerQuery = playerQuery.Where(player => player.VipId == request.VipId);

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                playerQuery = playerQuery.Where(player => player.Phone == request.Phone);
            }

            var playerIpQuery = from record in _tgsRepository.PlayerIpRecords.AsQueryable()
                                where record.IpType == 1
                                select record;

            var query = from player in playerQuery
                        join brand in queryableBrands
                            on player.BrandId equals brand.Id
                        join ipRecord in playerIpQuery
                            on player.PlayerCode equals ipRecord.PlayerCode into gj
                        from ipRecord in gj.DefaultIfEmpty()
                        select new PlayersModel
                        {
                            PlayerCode = player.PlayerCode,
                            Username = player.Username,
                            Balance = player.Wallets.FirstOrDefault().Balance,
                            VipLevel = player.Vip.Level,
                            Status = player.Status,
                            RegisteredTime = ipRecord.RecordOn,
                            RegisterHost = ipRecord.Host,
                            AgentName = player.Agent.Username
                        };

            if (!string.IsNullOrWhiteSpace(request.RegisterHost))
                query = query.Where(row => row.RegisterHost.ToLower() == request.RegisterHost.ToLower());

            if (request.BeginRegisteredOn != null && request.EndRegisteredOn != null)
                query = query.Where(pm => pm.RegisteredTime > request.BeginRegisteredOn
                    && pm.RegisteredTime <= request.EndRegisteredOn);

            return query.CreatePagedResponseAsync(request);
        }
    }
}
