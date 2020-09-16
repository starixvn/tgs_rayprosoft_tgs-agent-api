using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private Task<decimal> AtomicUpdateBalanceAsync(ITgsRepository repo, string playerCode, Guid walletId, decimal amount, DateTimeOffset updatedOn)
        {
            const string sql = "usp_Wallet_UpdateWalletBalance";
            return repo.SqlQuerySingleAsync<decimal>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@walletId",
                    Value = walletId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@amount",
                    Value = amount,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@lastUpdatedOn",
                    Value = updatedOn,
                    DbType = DbType.DateTimeOffset
                }
            });
        }

        private Task InsertNewRoundAsync(ITgsRepository repo, RoundData roundData)
        {
            const string sql = "usp_Round_InsertRound";
            return repo.SqlQuerySingleAsync<decimal>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@roundId",
                    Value = roundData.Id,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@externalRoundId",
                    Value = roundData.ExternalRoundId,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@walletId",
                    Value = roundData.WalletId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@gameId",
                    Value = roundData.GameId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@gameName",
                    Value = roundData.GameName,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@gameType",
                    Value = roundData.GameType,
                    DbType = DbType.Int16
                },
                new()
                {
                    ParameterName = "@gameProviderId",
                    Value = roundData.GameProviderId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@gameProviderCode",
                    Value = roundData.GameProviderCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@gameProviderName",
                    Value = roundData.GameProviderName,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = roundData.PlayerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@playerType",
                    Value = roundData.PlayerType,
                    DbType = DbType.Int16
                },
                new()
                {
                    ParameterName = "@currencyCode",
                    Value = roundData.CurrencyCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@platformType",
                    Value = roundData.PlatformType,
                    DbType = DbType.Int16
                },
                new()
                {
                    ParameterName = "@status",
                    Value = roundData.Status,
                    DbType = DbType.Int16
                },
                new()
                {
                    ParameterName = "@gameProviderConfigurationId",
                    Value = roundData.GameProviderConfigurationId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@licenseeId",
                    Value = roundData.LicenseeId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@licenseeCode",
                    Value = roundData.LicenseeCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@licenseeName",
                    Value = roundData.LicenseeName,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@createdOn",
                    Value = roundData.CreatedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@closedOn",
                    Value = roundData.ClosedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@updatedOn",
                    Value = roundData.UpdatedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@externalCreatedOn",
                    Value = roundData.ExternalCreatedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@externalClosedOn",
                    Value = roundData.ExternalClosedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@roundDetail",
                    Value = roundData.RoundDetail,
                    DbType = DbType.String
                },
            });
        }
                
        private Task UpdateRoundAsync(ITgsRepository repo, RoundData roundData)
        {
             const string sql = "usp_Round_UpdateRound";
            return repo.SqlQuerySingleAsync<decimal>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@roundId",
                    Value = roundData.Id,
                    DbType = DbType.Guid
                },               
                new()
                {
                    ParameterName = "@status",
                    Value = roundData.Status,
                    DbType = DbType.Int16
                },
                new()
                {
                    ParameterName = "@closedOn",
                    Value = roundData.ClosedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@updatedOn",
                    Value = roundData.UpdatedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@externalClosedOn",
                    Value = roundData.ExternalClosedOn,
                    DbType = DbType.DateTimeOffset
                },
                new()
                {
                    ParameterName = "@roundDetail",
                    Value = roundData.RoundDetail,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@betAmount",
                    Value = roundData.BetAmount,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@betAmount",
                    Value = roundData.BetAmount,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@tieBetAdjustment",
                    Value = roundData.TieBetAdjustment,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@commssion",
                    Value = roundData.Commission,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@isReopen",
                    Value = roundData.IsReopen,
                    DbType = DbType.Boolean
                },
                new()
                {
                    ParameterName = "@isClosed",
                    Value = roundData.IsClosed,
                    DbType = DbType.Boolean
                },
            });
        }

        private Task InsertGameActionAsync(ITgsRepository repo, GameActionData gameActionData)
        {
            const string sql = "usp_GameAction_InsertGameAction";
            return repo.SqlQuerySingleAsync<decimal>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameActionId",
                    Value = gameActionData.Id,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@gameProviderConfigurationId",
                    Value = gameActionData.GameProviderConfigurationId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@walletId",
                    Value = gameActionData.WalletId,
                    DbType = DbType.Guid
                }, 
                new()
                {
                    ParameterName = "@roundId",
                    Value = gameActionData.RoundId,
                    DbType = DbType.Guid
                },  
                new()
                {
                    ParameterName = "@externalTransactionId",
                    Value = gameActionData.ExternalTransactionId,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@type",
                    Value = gameActionData.Type,
                    DbType = DbType.Int64
                },
                new()
                {
                    ParameterName = "@amount",
                    Value = gameActionData.Amount,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@turnover",
                    Value = gameActionData.Turnover,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@winLoss",
                    Value = gameActionData.WinLoss,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@unsettledBets",
                    Value = gameActionData.UnsettledBets,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@balance",
                    Value = gameActionData.Balance,
                    DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@betType",
                    Value = gameActionData.BetType,
                    DbType = DbType.String
                }, 
                new()
                {
                    ParameterName = "@externalBetId",
                    Value = gameActionData.ExternalBetId,
                    DbType = DbType.String
                }, 
                new()
                {
                    ParameterName = "@externalGameActionGroupId",
                    Value = gameActionData.ExternalGameActionGroupId,
                    DbType = DbType.String
                }, 
                new()
                {
                    ParameterName = "@referencedGameActionId",
                    Value = gameActionData.ReferencedGameActionId,
                    DbType = DbType.Guid
                }, 
                new()
                {
                    ParameterName = "@gameActionBatchId",
                    Value = gameActionData.GameActionBatchId,
                    DbType = DbType.Guid
                }, 
                new()
                {
                    ParameterName = "@recordedOn",
                    Value = gameActionData.RecordedOn,
                    DbType = DbType.DateTimeOffset
                }, 
                new()
                {
                    ParameterName = "@sentOn",
                    Value = gameActionData.SentOn,
                    DbType = DbType.DateTimeOffset
                }, 
                new()
                {
                    ParameterName = "@isClosingRound",
                    Value = gameActionData.IsClosingRound,
                    DbType = DbType.Boolean
                }, 
                new()
                {
                    ParameterName = "@closingRoundExplicitRequest,",
                    Value = gameActionData.ClosingRoundExplicitRequest,
                    DbType = DbType.Boolean
                }, 
                new()
                {
                    ParameterName = "@licenseeId",
                    Value = gameActionData.LicenseeId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = gameActionData.PlayerCode,
                    DbType = DbType.String
                }, 
                new()
                {
                    ParameterName = "@playerIp",
                    Value = gameActionData.PlayerIp,
                    DbType = DbType.String
                },                
                new()
                {
                    ParameterName = "@description",
                    Value = gameActionData.Description,
                    DbType = DbType.String
                }                 
            });
        }
    }
}