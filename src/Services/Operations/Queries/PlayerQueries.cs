using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.CustomQueryParameters;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IPlayerQueries
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);

        Task<GetPlayerLoginRecordResult[]> GetPlayerLoginsRecordsInOneDayAsync(string playerCode,
            params LoginStatus[] statuses);

        Task<CachedPlayerLoginRecord> GetPlayerLoginRecordAsync(Guid loginRecordId);

        Task<string> GetPlayerCodeByUsernameAsync(Guid brandId, string username);

        Task<Player> GetPlayerByCodeAsync(string playerCode);

        Task<(byte[] salt, byte[] password)> GetSaltAndPasswordByPlayerCodeAsync(string playerCode);
    }

    public sealed class PlayerQueries : IPlayerQueries
    {
        private readonly ITgsRepository _repository;

        public PlayerQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<CachedPlayer> IPlayerQueries.GetCachedPlayerAsync(string playerCode)
        {
            const string spName = "dbo.player_get_player_for_cache";
            return _repository.SqlQuerySingleAsync<CachedPlayer>(spName,
                parameters: new Dictionary<string, object> {{":player_code", playerCode}});
        }

        async Task<GetPlayerLoginRecordResult[]> IPlayerQueries.GetPlayerLoginsRecordsInOneDayAsync(string playerCode,
            LoginStatus[] statuses)
        {
            const string spName = "dbo.player_get_player_login_records";
            var results = await _repository
                .SqlQueryMultipleAsync<GetPlayerLoginRecordResult>(spName, parameters:
                    new Dictionary<string, object>
                    {
                        {":player_code", playerCode},
                        {":statuses", new ArrayParameter<int>(statuses.Select(s => (int)s).ToArray())},
                        {":from", DateTimeOffset.UtcNow.AddDays(-1)}
                    });

            return results.ToArray();
        }

        Task<CachedPlayerLoginRecord> IPlayerQueries.GetPlayerLoginRecordAsync(Guid loginRecordId) =>
            _repository.PlayerLoginRecords.Where(r => r.Id == loginRecordId)
                .Select(r => new CachedPlayerLoginRecord
                {
                    PlayerCode = r.PlayerCode,
                    PlatformType = r.PlatformType,
                    Status = r.Status,
                    ExpiredOn = r.ExpiredOn
                }).FirstOrDefaultAsync();

        Task<string> IPlayerQueries.GetPlayerCodeByUsernameAsync(Guid brandId, string username) =>
            _repository.Players
                .Where(p => p.BrandId == brandId &&
                    p.Username.ToLower().Equals(username.ToLower()))
                .Select(p => p.PlayerCode)
                .FirstOrDefaultAsync();

        Task<Player> IPlayerQueries.GetPlayerByCodeAsync(string playerCode) =>
            _repository.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlayerCode == playerCode);

        Task<(byte[] salt, byte[] password)> IPlayerQueries.GetSaltAndPasswordByPlayerCodeAsync(string playerCode) =>
            _repository.Players.Where(p => p.PlayerCode == playerCode)
                .Select(p => new Tuple<byte[], byte[]>(p.Salt, p.Password).ToValueTuple())
                .FirstOrDefaultAsync();
    }
}