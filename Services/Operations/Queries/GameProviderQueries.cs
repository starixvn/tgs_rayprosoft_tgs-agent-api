using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Services.Operations.Extensions;

namespace TGS.Services.Operations.Queries
{
    public interface IGameProviderQueries
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);
    }

    public sealed class GameProviderQueries : IGameProviderQueries
    {
        private readonly ITgsRepository _repository;

        public GameProviderQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Guid> IGameProviderQueries.GetGameProviderIdByCodeAsync(string gameProviderCode)
        {
            const string spName = "usp_GameProvider_GetGameProviderIdByCode";
            return _repository.SqlQueryAsync<Guid>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderCode",
                Value = gameProviderCode,
                DbType = DbType.String
            });
        }
    }
}
