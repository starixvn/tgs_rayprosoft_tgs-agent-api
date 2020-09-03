using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IPlayerQueries
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);

        Task<GetPlayerLoginRecordResult> GetPlayerLatestEnabledLoginRecordAsync(string playerCode, PlatformType? platform);
    }

    public sealed class PlayerQueries : IPlayerQueries
    {
        private readonly TgsRepository _repository;

        public PlayerQueries(TgsRepository repository)
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

        Task<GetPlayerLoginRecordResult> IPlayerQueries.GetPlayerLatestEnabledLoginRecordAsync(string playerCode, PlatformType? platform)
        {
            const string spName = "usp_Player_GetPlayerLatestEnabledLoginRecord";
            return _repository.SqlQuerySingleAsync<GetPlayerLoginRecordResult>(spName, parameters:
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
                        ParameterName = "@platform",
                        Value = platform,
                        DbType = DbType.Int16
                    }
                });
        }
    }
}