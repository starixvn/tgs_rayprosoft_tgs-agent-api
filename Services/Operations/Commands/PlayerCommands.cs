using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
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
        private readonly ITgsRepository _repository;

        public PlayerCommands(
            ITgsRepository repository,
            ITgsLog log)
        {
            _repository = repository;
            _log = log;
        }

        async Task<string> IPlayerCommands.CreatePlayerAsync(CreatePlayerData data)
        {
            var retries = 0;

            var username = data.Username;
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
                        WithdrawPassword = data.WithdrawPassword,
                        WithdrawSalt = data.WithdrawSalt,
                        InvitationCode = InvitationCodeGenerator.NewId(),
                        BrandId = data.BrandId,
                        DefaultWalletId = walletId,
                        DefaultWalletCurrencyCode = data.CurrencyCode,
                        ParentInvitationCode = data.ParentInvitationCode,
                        RegisteredOn = createdOn,
                        LastActiveOn = createdOn,
                        Status = PlayerStatus.Enabled,
                        Type = data.Type,
                        BetLimitId = data.BetLimitId,
                        HasAgent = data.HasAgent,
                        UpdatedByUsername = username,
                        UpdatedOn = createdOn,
                        Wallets = new HashSet<Wallet>
                        {
                            new()
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

                    await _repository.Players.AddAsync(player);
                    await _repository.SaveChangesAsync();

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
        }

        Task IPlayerCommands.UpdatePlayerAsync(UpdatePlayerData data)
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

            _repository.Players.Attach(player);
            _repository.Entry(player).Property(nameof(player.RealName)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.Email)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.Phone)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.QQ)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.Wechat)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        Task IPlayerCommands.ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            var player = new Player
            {
                PlayerCode = data.PlayerCode,
                Salt = data.Salt,
                Password = data.Password,
                UpdatedOn = data.UpdatedOn
            };

            _repository.Players.Attach(player);
            _repository.Entry(player).Property(nameof(player.Salt)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.Password)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        async Task<PlayerLoginRecord> IPlayerCommands.PlayerLoginAsync(PlayerLoginData data)
        {
            var loginRecord = new PlayerLoginRecord
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

            await _repository.PlayerLoginRecords.AddAsync(loginRecord);
            await _repository.SaveChangesAsync();

            return loginRecord;
        }

        Task IPlayerCommands.PlayerLogoutAsync(PlayerLogoutData data) =>
            DeauthorizePlayerAsync(_repository, data.PlayerCode, data.LoginRecordId);

        #region Private Methods

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

        private static Task<int> DeauthorizePlayerAsync(ITgsRepository repo, string playerCode,
            PlatformType? platformType)
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