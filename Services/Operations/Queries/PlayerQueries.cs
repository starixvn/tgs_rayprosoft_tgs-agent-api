using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
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

        Task<string> GetPlayerCodeByUsernameAsync(Guid licenseeId, string username);

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
            const string spName = "usp_Player_GetPlayerForCache";
            return _repository.SqlQuerySingleAsync<CachedPlayer>(spName, parameters: new SqlParameter
            {
                ParameterName = "@playerCode",
                Value = playerCode,
                DbType = DbType.String
            });
        }

        async Task<GetPlayerLoginRecordResult[]> IPlayerQueries.GetPlayerLoginsRecordsInOneDayAsync(string playerCode,
            LoginStatus[] statuses)
        {
            const string spName = "usp_Player_GetPlayerLoginRecords";
            var results = await _repository.SqlQueryMultipleAsync<GetPlayerLoginRecordResult>(spName, parameters:
                new SqlParameter[]
                {
                    new()
                    {
                        ParameterName = "@playerCode",
                        Value = playerCode,
                        DbType = DbType.String
                    },
                    new()
                    {
                        ParameterName = "@statuses",
                        Value = statuses.Select(s => new {Value = (int) s}).ToDataTable(),
                        SqlDbType = SqlDbType.Structured
                    },
                    new()
                    {
                        ParameterName = "@from",
                        Value = DateTimeOffset.UtcNow.AddDays(-1),
                        DbType = DbType.DateTimeOffset
                    }
                });

            return results.ToArray();
        }

        Task<string> IPlayerQueries.GetPlayerCodeByUsernameAsync(Guid licenseeId, string username) =>
            _repository.Players
                .Where(p => p.LicenseeId == licenseeId &&
                    p.Username == username)
                .Select(p => p.PlayerCode)
                .FirstOrDefaultAsync();

        Task<Player> IPlayerQueries.GetPlayerByCodeAsync(string playerCode) =>
            _repository.Players
                .FirstOrDefaultAsync(p => p.PlayerCode == playerCode);

        Task<(byte[] salt, byte[] password)> IPlayerQueries.GetSaltAndPasswordByPlayerCodeAsync(string playerCode) =>
            _repository.Players.Where(p => p.PlayerCode == playerCode)
                .Select(p => new Tuple<byte[], byte[]>(p.Salt, p.Password).ToValueTuple())
                .FirstOrDefaultAsync();
    }
}