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
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using Tgs.Logging;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Operations.Commands
{
    public sealed class PlayerCommands : IPlayerCommands
    {
        private const int _RetryCount = 3;
        private readonly ITgsLog _log;
        private readonly ITgsRepository _repository;

        public PlayerCommands(
            ITgsRepository repository,
            ITgsLog log)
        {
            _repository = repository;
            _log = log;
        }

        public async Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            var player = await _repository.Players.FindAsync(data.PlayerCode).ConfigureAwait(false);

            player.Salt = data.Salt;
            player.Password = data.Password;
            player.UpdatedOn = data.UpdatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<string> CreatePlayerAsync(CreatePlayerData data)
        {
            var retries = 0;

            var username = data.UserName;
            var walletId = GuidExtensions.NewSequentialGuid();
            var createdOn = DateTimeOffset.UtcNow;
            var playerCode = default(string);

            var vip = await _repository.VipLevels.FirstOrDefaultAsync(x => x.BrandId == data.BrandId && x.Level == 1);

            while (retries < _RetryCount)
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
                        LastActiveOn = createdOn,
                        Status = data.PlayerStatus,
                        Type = data.Type,
                        BetLimitId = data.BetLimitId,
                        AgentId = data.AgentId,
                        UpdatedByUsername = username,
                        UpdatedOn = createdOn,
                        VipId = vip.Id,
                        Wallets = new HashSet<Wallet>
                        {
                            new()
                            {
                                Id = walletId,
                                PlayerCode = playerCode,
                                CurrencyCode = data.CurrencyCode,
                                ExternalCurrencyCode = data.CurrencyCode,
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
                        $"Exception occurs when creating player. This is attempt number {retries} of {_RetryCount}.";

                    _log.LogError(err, e);

                    if (retries == _RetryCount)
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

        public Task DeauthorizePlayerAsync(string playerCode, Guid loginRecordId)
        {
            const string sql = "call dbo.player_deauthorize_by_login_record_id(:player_code, :login_record_id)";

            return _repository.ExecuteSqlCommandAsync(sql,
                CommandType.Text,
                new Dictionary<string, object>
                {
                    {
                        ":player_code", playerCode
                    },
                    {
                        ":login_record_id", loginRecordId
                    }
                });
        }

        public Task DeauthorizePlayerAsync(string playerCode, PlatformType? platformType)
        {
            const string sql = "call dbo.player_deauthorize_by_platform_type";

            return _repository.ExecuteSqlCommandAsync(sql,
                CommandType.Text,
                new Dictionary<string, object>
                {
                    {
                        ":player_code", playerCode
                    },
                    {
                        ":platform_type", platformType
                    }
                });
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

        public async Task<int> SetEmailAsync(string playerCode, string email, DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.Email = email;
            player.UpdatedOn = updatedOn;

            return await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetIdentityAsync(
            string playerCode, string realName, string identity, bool isAuthenticate,
            DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.RealName = realName;
            player.IdNumber = identity;
            player.IdentityAuthentication = isAuthenticate;
            player.UpdatedOn = updatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetPayPasswordAsync(
            string playerCode, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.WithdrawSalt = newSalt;
            player.WithdrawPassword = newPassword;
            player.UpdatedOn = updatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetPhoneAsync(string playerCode, string phone, DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.Phone = phone;
            player.PhoneAuthentication = true;
            player.UpdatedOn = updatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetQqAsync(string playerCode, string qqId, DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.QQ = qqId;
            player.UpdatedOn = updatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetWeChatAsync(string playerCode, string weChatId, DateTimeOffset updatedOn)
        {
            var player = await _repository.Players.FindAsync(playerCode).ConfigureAwait(false);

            player.Wechat = weChatId;
            player.UpdatedOn = updatedOn;

            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}