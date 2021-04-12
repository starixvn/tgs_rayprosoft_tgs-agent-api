using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Player;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPlayerService
    {
        Task<Player> GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery = null);

        IQueryable<Player> GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>> preQuery = null,
            bool tracking = false);

        IQueryable<PlayerAuditLog> GetPlayerOperationQueries(
            Func<IQueryable<PlayerAuditLog>, IQueryable<PlayerAuditLog>> preQuery = null,
            bool tracking = false);

        Task UpdatePlayerAsync(Player player, UpdatePlayerRequest request);
        Task<bool> ExistsAsync(Expression<Func<Player, bool>> predicate);
    }

    public class PlayerService : BaseRepositoryOperations<ITgsRepository>, IPlayerService
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;
        private readonly ITransactionManager _transactionManager;

        public PlayerService(ITgsRepository tgsRepository, ITransactionManager transactionManager,
            IPasswordHandler passwordHandler, IRedisCacheManager cacheManager) : base(
            tgsRepository)
        {
            _transactionManager = transactionManager;
            _repository = tgsRepository;
            _passwordHandler = passwordHandler;
            _cacheManager = cacheManager;
        }

        Task<Player> IPlayerService.GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Player> IPlayerService.GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IPlayerService.UpdatePlayerAsync(
            Player player, UpdatePlayerRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            return _transactionManager.ExecuteTransactionAsync(() => _repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
                    var updatePlayerType = new[] {"info", "contact", "password"};

                    if (request.Type == updatePlayerType[0])
                    {
                        player.RealName = request.RealName;
                        player.VipId = request.VipId;
                        player.Status = request.Status;
                    }

                    if (request.Type == updatePlayerType[1])
                    {
                        player.Phone = request.Phone;
                        player.Email = request.Email;
                        player.QQ = request.QQ;
                        player.Wechat = request.Wechat;
                    }

                    if (request.Type == updatePlayerType[2])
                    {
                        var (salt, pwd) = _passwordHandler.GenerateCredential(request.Password);
                        player.Password = pwd;
                        player.Salt = salt;

                        await _cacheManager.ClearCacheAsync(RedisCacheType.Player, request.PlayerCode);
                    }

                    player.UpdatedByUsername = request.EditorUsername;
                    player.UpdatedOn = DateTimeOffset.Now;

                    var auditLog = new PlayerAuditLog
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        ModuleName = "Player",
                        CreatedByUsername = request.EditorUsername,
                        CreatedOn = DateTimeOffset.Now,
                        Operation = $"Update Player {request.Type}",
                        Username = request.PlayerCode
                    };

                    await _cacheManager.ClearCacheAsync(RedisCacheType.Player, request.PlayerCode);

                    await UpdateEntityAsync(player);
                    await CreateEntityAsync(auditLog);

                    return Task.CompletedTask;
                });
        }

        Task<bool> IPlayerService.ExistsAsync(Expression<Func<Player, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<PlayerAuditLog> IPlayerService.GetPlayerOperationQueries(
            Func<IQueryable<PlayerAuditLog>, IQueryable<PlayerAuditLog>> preQuery, bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}