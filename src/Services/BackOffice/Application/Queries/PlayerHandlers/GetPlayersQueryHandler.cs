using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers;

public class GetPlayersQueryHandler : Handler<GetPlayersRequest, PagedResponse<PlayersModel>>
{
    private readonly ITgsRepository _tgsRepository;

    public GetPlayersQueryHandler(
        ITgsRepository tgsRepository,
        IHttpContextAccessor accessor) :
        base(accessor)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
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
            .AsQueryable();

        if (request.BrandId != null)
            playerQuery = playerQuery.Where(player => player.BrandId == request.BrandId);

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            playerQuery = playerQuery.Where(player =>
                EF.Functions.ILike(player.Username, $"{request.Username}%"));
        }

        if (request.Status != null)
            playerQuery = playerQuery.Where(player => player.Status == request.Status);

        if (request.VipId != null)
            playerQuery = playerQuery.Where(player => player.VipId == request.VipId);

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            playerQuery = playerQuery.Where(player => player.Phone == request.Phone);
        }

        if (request.BeginRegisteredOn != null && request.EndRegisteredOn != null)
        {
            playerQuery = playerQuery.Where(p =>
                p.RegisteredOn > request.BeginRegisteredOn
                && p.RegisteredOn <= request.EndRegisteredOn);
        }

        if (!string.IsNullOrWhiteSpace(request.RealName))
        {
            playerQuery = playerQuery.Where(p =>
                EF.Functions.ILike(p.RealName, $"{request.RealName}%"));
        }

        var walletQuery =
            from wallet in _tgsRepository.Wallets.AsQueryable()
            join player in playerQuery on wallet.Id equals player.DefaultWalletId
            select new
            {
                wallet.Id,
                wallet.Balance
            };

        var playerIpQuery =
            from record in _tgsRepository.PlayerIpRecords.AsQueryable()
            join player in playerQuery on record.PlayerCode equals player.PlayerCode
            where record.IpType == 1
            select new
            {
                record.PlayerCode,
                record.Host
            };

        var playerIdentityQuery =
            from identity in _tgsRepository.PlayerIdentity.AsQueryable()
            join player in playerQuery on identity.PlayerCode equals player.PlayerCode
            select new
            {
                identity.PlayerCode,
                identity.PlayerAuthenticationFormId
            };

        var playerParentQuery =
            from player in _tgsRepository.Players.AsQueryable()
            join playerParent in _tgsRepository.PlayerParents
                on player.PlayerCode equals playerParent.PlayerCode into ppj
            from playerParent in ppj.DefaultIfEmpty()
            join player2 in _tgsRepository.Players.AsQueryable()
                on playerParent.ParentPlayerCode equals player2.PlayerCode into ppj2
            from player2 in ppj2.DefaultIfEmpty()
            select new
            {
                player.PlayerCode,
                playerParent.ParentPlayerCode,
                ParentUsername = player2.Username
            };

        var query =
                    from player in playerQuery
                    join brand in queryableBrands
                        on player.BrandId equals brand.Id
                    join wallet in walletQuery
                        on player.DefaultWalletId equals wallet.Id
                    join ipRecord in playerIpQuery
                        on player.PlayerCode equals ipRecord.PlayerCode into gj
                    from ipRecord in gj.DefaultIfEmpty()
                    join playerIdentity in playerIdentityQuery
                        on player.PlayerCode equals playerIdentity.PlayerCode into pij
                    from playerIdentity in pij.DefaultIfEmpty()
                    join playerParent in playerParentQuery
                        on player.PlayerCode equals playerParent.PlayerCode into ppj
                    from playerParent in ppj.DefaultIfEmpty()
                    select new
                    {
                        player.PlayerCode,
                        player.Username,
                        player.RealName,
                        wallet.Balance,
                        VipLevel = player.Vip.Level,
                        player.Status,
                        player.RegisteredOn,
                        RegisterHost = ipRecord.Host,
                        AgentName = player.Agent.Username,
                        IdentityFormId = playerIdentity.PlayerAuthenticationFormId,
                        ParentPlayerCode = playerParent.ParentPlayerCode,
                        ParentUsername = playerParent.ParentUsername
                    };

        if (!string.IsNullOrWhiteSpace(request.RegisterHost))
            query = query.Where(row => row.RegisterHost.ToLower() == request.RegisterHost.ToLower());

        if (!string.IsNullOrWhiteSpace(request.ParentUsername))
            query = query.Where(row => row.ParentUsername == request.ParentUsername);

        if (!string.IsNullOrWhiteSpace(request.AgentName))
            query = query.Where(row => row.AgentName == request.AgentName);

        return query.Select(row => new PlayersModel
        {
            PlayerCode = row.PlayerCode,
            Username = row.Username,
            RealName = row.RealName,
            Balance = row.Balance,
            VipLevel = row.VipLevel,
            Status = row.Status,
            RegisteredTime = row.RegisteredOn,
            RegisterHost = row.RegisterHost,
            AgentName = row.AgentName,
            IdentityFormId = row.IdentityFormId,
            ParentUsername = row.ParentUsername,
        }).CreatePagedResponseAsync(request);
    }
}
