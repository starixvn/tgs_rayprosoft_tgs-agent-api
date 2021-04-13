using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        Task UpdatePlayerIdNumberAsync(Player player);

        Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data);

        Task<PlayerLoginRecord> PlayerLoginAsync(PlayerLoginData data);

        Task DeauthorizePlayerAsync(string playerCode, Guid loginRecordId);

        Task DeauthorizePlayerAsync(string playerCode, PlatformType? platformType);

        Task SetPayPasswordAsync(string playerCode, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);

        Task SetEmailAsync(string playerCode, string email, DateTimeOffset updatedOn);
        Task SetPhoneAsync(string playerCode, string phone, DateTimeOffset updatedOn);

        Task SetIdentityAsync(
            string playerCode,
            string realName,
            string identity,
            bool isAuthenticate,
            DateTimeOffset updatedOn);

        Task SetQqAsync(string playerCode, string qqId, DateTimeOffset updatedOn);

        Task SetWeChatAsync(string playerCode, string weChatId, DateTimeOffset updatedOn);
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

        public async Task<string> CreatePlayerAsync(CreatePlayerData data)
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
                        BrandId = data.BrandId,
                        DefaultWalletId = walletId,
                        DefaultWalletCurrencyCode = data.CurrencyCode,
                        RegisteredOn = createdOn,
                        LastActiveOn = createdOn,
                        Status = PlayerStatus.Enabled,
                        Type = data.Type,
                        BetLimitId = data.BetLimitId,
                        AgentId = data.AgentId,
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
                catch (Exception e) when (e.InnerException?.InnerException is NpgsqlException exception)
                {
                    retries++;
                    var err =
                        $"Exception occurs when creating player. This is attempt number {retries} of {RetryCount}.";

                    _log.LogError(err, e);

                    if (retries == RetryCount)
                    {
                        throw new InvalidOperationException(err, e);
                    }

                    if (exception.ErrorCode == 2601) // 2601 is "unique index"
                    {
                        break;
                    }
                }
            }

            return playerCode;
        }

        public Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            var player = new Player
            {
                PlayerCode = data.PlayerCode, Salt = data.Salt, Password = data.Password, UpdatedOn = data.UpdatedOn
            };

            _repository.Players.Attach(player);
            _repository.Entry(player).Property(nameof(player.Salt)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.Password)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public async Task<PlayerLoginRecord> PlayerLoginAsync(PlayerLoginData data)
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

        public Task DeauthorizePlayerAsync(string playerCode, Guid loginRecordId)
        {
            const string sql = "call dbo.player_deauthorize_by_login_record_id(:player_code, :login_record_id)";
            return _repository.ExecuteSqlCommandAsync(sql, CommandType.Text,
                new Dictionary<string, object> {{":player_code", playerCode}, {":login_record_id", loginRecordId}});
        }

        public Task DeauthorizePlayerAsync(string playerCode, PlatformType? platformType)
        {
            const string sql = "call dbo.player_deauthorize_by_platform_type";
            return _repository.ExecuteSqlCommandAsync(sql, CommandType.Text,
                new Dictionary<string, object> {{":player_code", playerCode}, {":platform_type", platformType}});
        }

        public Task UpdatePlayerIdNumberAsync(Player player)
        {
            _repository.Entry(player).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }

        public Task SetPayPasswordAsync(string playerCode, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn)
        {
            var player = new Player
            {
                PlayerCode = playerCode,
                WithdrawPassword = newPassword,
                WithdrawSalt = newSalt,
                UpdatedOn = updatedOn
            };

            _repository.Players.Attach(player);
            _repository.Entry(player).Property(nameof(player.WithdrawSalt)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.WithdrawPassword)).IsModified = true;
            _repository.Entry(player).Property(nameof(player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public Task SetEmailAsync(string playerCode, string email, DateTimeOffset updatedOn)
        {
            var updateInfo = new Player {PlayerCode = playerCode, Email = email, UpdatedOn = updatedOn};

            _repository.Players.Attach(updateInfo);
            _repository.Entry(updateInfo).Property(nameof(Player.Email)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public Task SetPhoneAsync(string playerCode, string phone, DateTimeOffset updatedOn)
        {
            var updateInfo = new Player
            {
                PlayerCode = playerCode, Phone = phone, PhoneAuthentication = true, UpdatedOn = updatedOn
            };

            _repository.Players.Attach(updateInfo);
            _repository.Entry(updateInfo).Property(nameof(Player.Phone)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.PhoneAuthentication)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public Task SetIdentityAsync(string playerCode, string realName, string identity, bool isAuthenticate,
            DateTimeOffset updatedOn)
        {
            var updateInfo = new Player
            {
                PlayerCode = playerCode,
                RealName = realName,
                IdNumber = identity,
                IdentityAuthentication = isAuthenticate,
                UpdatedOn = updatedOn
            };

            _repository.Players.Attach(updateInfo);
            _repository.Entry(updateInfo).Property(nameof(Player.RealName)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.IdNumber)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.IdentityAuthentication)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public Task SetQqAsync(string playerCode, string qqId, DateTimeOffset updatedOn)
        {
            var updateInfo = new Player {PlayerCode = playerCode, QQ = qqId, UpdatedOn = updatedOn};

            _repository.Players.Attach(updateInfo);
            _repository.Entry(updateInfo).Property(nameof(Player.QQ)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }

        public Task SetWeChatAsync(string playerCode, string weChatId, DateTimeOffset updatedOn)
        {
            var updateInfo = new Player {PlayerCode = playerCode, Wechat = weChatId, UpdatedOn = updatedOn};

            _repository.Players.Attach(updateInfo);
            _repository.Entry(updateInfo).Property(nameof(Player.Wechat)).IsModified = true;
            _repository.Entry(updateInfo).Property(nameof(Player.UpdatedOn)).IsModified = true;

            return _repository.SaveChangesAsync();
        }
    }
}