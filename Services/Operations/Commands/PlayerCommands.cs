using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
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

        Task UpdatePlayerAsync(UpdatePlayerData data);

        Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data);

        Task<PlayerLoginRecord> PlayerLoginAsync(PlayerLoginData data);

        Task PlayerLogoutAsync(PlayerLogoutData data);
    }

    public sealed class PlayerCommands : IPlayerCommands
    {
        private const int RetryCount = 3;
        private readonly ITgsLog _log;
        private readonly Func<ITgsRepository> _repositoryFactory;
        private readonly ITransactionManager _transactionManager;

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
                async repo =>
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
                                BrandId = data.BrandId,
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

                            await repo.Players.AddAsync(player);

                            CreateAuditLog(repo, username, $"Create Player '{username}'", createdOn);

                            await repo.SaveChangesAsync();

                            break;
                        }
                        catch (Exception e) when (e.InnerException?.InnerException is SqlException exception)
                        {
                            retries++;
                            var err =
                                $"Exception occurs when creating player. This is attempt number {retries} of {RetryCount}.";

                            _log.LogError(err, e);

                            if (retries == RetryCount)
                            {
                                throw new InvalidOperationException(err, e);
                            }

                            if (exception.Number == 2601) // 2601 is "unique index"
                            {
                                break;
                            }
                        }
                    }

                    return playerCode;
                });
        }

        async Task IPlayerCommands.UpdatePlayerAsync(UpdatePlayerData data)
        {
            var username = data.Username;

            _ = await _transactionManager.ExecuteTransactionAsync(
                _repositoryFactory,
                IsolationLevel.ReadCommitted,
                username,
                async repo =>
                {
                    var player = new Player
                    {
                        PlayerCode = data.PlayerCode,
                        RealName = data.RealName,
                        Email = data.Email,
                        Phone = data.Phone,
                        QQ = data.QQ,
                        Wechat = data.Wechat,
                        UpdatedOn = data.UpdatedOn
                    };

                    repo.Players.Attach(player);
                    repo.Entry(player).Property(nameof(player.RealName)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.Email)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.Phone)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.QQ)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.Wechat)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

                    CreateAuditLog(repo, username, "Update player information", data.UpdatedOn);

                    await repo.SaveChangesAsync();

                    return Task.CompletedTask;
                });
        }

        async Task IPlayerCommands.ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            var username = data.Username;

            _ = await _transactionManager.ExecuteTransactionAsync(
                _repositoryFactory,
                IsolationLevel.ReadCommitted,
                username,
                async repo =>
                {
                    var player = new Player
                    {
                        PlayerCode = data.PlayerCode,
                        Salt = data.Salt,
                        Password = data.Password,
                        UpdatedOn = data.UpdatedOn
                    };

                    repo.Players.Attach(player);
                    repo.Entry(player).Property(nameof(player.Salt)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.Password)).IsModified = true;
                    repo.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

                    CreateAuditLog(repo, username, "Update player password", data.UpdatedOn);

                    await repo.SaveChangesAsync();

                    return Task.CompletedTask;
                });
        }

        async Task<PlayerLoginRecord> IPlayerCommands.PlayerLoginAsync(PlayerLoginData data)
        {
            var username = data.Username;

            var loginRecord = await _transactionManager.ExecuteTransactionAsync(
                _repositoryFactory,
                IsolationLevel.ReadCommitted,
                username,
                async repo =>
                {
                    await DeauthorizePlayerAsync(repo, data.PlayerCode, data.PlatformType);

                    var record = new PlayerLoginRecord
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        PlayerCode = data.PlayerCode,
                        BrandId = data.BrandId,
                        Status = LoginStatus.Enabled,
                        PlatformType = data.PlatformType,
                        IpAddress = data.IpAddress,
                        CreatedOn = data.CreatedOn,
                        ExpiredOn = data.ExpiredOn
                    };

                    await repo.PlayerLoginRecords.AddAsync(record);

                    CreateAuditLog(repo, username, $"Player '{username}' login.", data.CreatedOn);

                    await repo.SaveChangesAsync();

                    return record;
                });

            return loginRecord;
        }

        async Task IPlayerCommands.PlayerLogoutAsync(PlayerLogoutData data)
        {
            var username = data.Username;

            _ = await _transactionManager.ExecuteTransactionAsync(
                _repositoryFactory,
                IsolationLevel.ReadCommitted,
                username,
                async repo =>
                {
                    await DeauthorizePlayerAsync(repo, data.PlayerCode, data.LoginRecordId);

                    CreateAuditLog(repo, username, $"Player '{username}' logout.", data.UpdatedOn);

                    await repo.SaveChangesAsync();

                    return Task.CompletedTask;
                });
        }

        #region Private Methods

        private static void CreateAuditLog(ITgsRepository repo, string username, string operation,
            DateTimeOffset createdOn)
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

        private static Task<int> DeauthorizePlayerAsync(ITgsRepository repo, string playerCode, Guid loginRecordId)
        {
            const string sql = "usp_Player_DeauthorizeByLoginRecordId";
            return repo.ExecuteSqlCommandAsync(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@loginRecordId",
                    Value = loginRecordId,
                    DbType = DbType.Guid
                }
            });
        }

        private static Task<int> DeauthorizePlayerAsync(ITgsRepository repo, string playerCode, PlatformType? platformType)
        {
            const string sql = "usp_Player_DeauthorizeByPlatform";
            return repo.ExecuteSqlCommandAsync(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@platformType",
                    Value = platformType,
                    DbType = DbType.Int16
                }
            });
        }

        #endregion Private Methods
    }
}