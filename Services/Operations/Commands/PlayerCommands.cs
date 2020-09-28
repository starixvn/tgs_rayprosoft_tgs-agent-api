using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Operations.Commands
{
    public interface IPlayerCommands
    {
        Task<string> CreatePlayerAsync(CreatePlayerData data);
        Task PlayerLoginAsync(PlayerLoginData data);
    }

    public sealed class PlayerCommands : IPlayerCommands
    {
        private readonly Func<ITgsRepository> _repositoryFactory;
        private readonly ITransactionManager _transactionManager;
        private readonly ITgsLog _log;

        private const int RetryCount = 3;

        public PlayerCommands(
            Func<ITgsRepository> repositoryFactory,
            ITransactionManager transactionManager,
            ITgsLog log)
        {
            _repositoryFactory = repositoryFactory;
            _transactionManager = transactionManager;
            _log = log;
        }

        async Task<string> IPlayerCommands.CreatePlayerAsync(CreatePlayerData data)
        {
            var retries = 0;
            var username = data.Username;

            return await _transactionManager.ExecuteTransactionAsync(
               _repositoryFactory,
              IsolationLevel.ReadCommitted,
              username,
              execute: async (repo, trans) =>
              {
                  var walletId = GuidExtensions.NewSequentialGuid();
                  var createdOn = DateTimeOffset.UtcNow;
                  var playerCode = default(string);

                  while (retries < RetryCount)
                  {
                      playerCode = PlayerCodeGenerator.NewId();

                      try
                      {
                          var player = new Player
                          {
                              PlayerCode = playerCode,
                              Username = username,
                              Password = data.Password,
                              Salt = data.Salt,
                              InvitationCode = StringGenerator.GenerateAlphanumericString(6),
                              LicenseeId = data.LicenseeId,
                              CultureCode = data.CultureCode,
                              DefaultWalletId = walletId,
                              DefaultWalletCurrencyCode = data.CurrencyCode,
                              ParentInvitationCode = data.ParentInvitationCode,
                              RegisteredOn = createdOn,
                              LastActiveOn = createdOn,
                              Status = PlayerStatus.Enabled,
                              Type = data.Type,
                              BetLimitGroupId = data.BetLimitGroup,
                              HasAgent = data.HasAgent,
                              UpdatedByUsername = username,
                              UpdatedOn = createdOn,
                              Wallets = new HashSet<Wallet>
                              {
                                  new Wallet
                                  {
                                      Id = walletId,
                                      PlayerCode = playerCode,
                                      CurrencyCode = data.CurrencyCode,
                                      ExternalCurrencyCode = data.ExternalCurrencyCode,
                                      Balance = 0,
                                      CreatedOn = createdOn,
                                      UpdatedOn = createdOn
                                  }
                              }
                          };

                          repo.Players.Add(player);

                          CreateAuditLog(repo, username, $"Create Player '{username}'", createdOn);

                          await repo.SaveChangesAsync()
                                    .ConfigureAwait(false);

                          break;
                      }
                      catch (Exception e) when (e.InnerException?.InnerException is SqlException)
                      {
                          retries++;
                          var err =
                              $"Exception occurs when creating player. This is attempt number {retries} of {RetryCount}.";

                          _log.LogError(err, e);

                          if (retries == RetryCount)
                          {
                              throw new InvalidOperationException(err, e);
                          }

                          var exception = (SqlException)e.InnerException.InnerException;
                          if (exception.Number == 2601) // 2601 is "unique index"
                          {
                              continue;
                          }
                      }
                  }

                  return playerCode;
              });
        }

        Task IPlayerCommands.PlayerLoginAsync(PlayerLoginData data)
        {
            throw new NotImplementedException();
        }


        #region Private Methods

        private static void CreateAuditLog(ITgsRepository repo, string username, string operation, DateTimeOffset createdOn)
        {
            repo.AuditLogs.Add(new PlayerAuditLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ModuleName = "Player",
                CreatedByUsername = username,
                CreatedOn = createdOn,
                Operation = operation,
                Username = username
            });
        }

        #endregion
    }
}
