using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Agent.ApplicationServices.Player;

namespace Agent.UnitTests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public async Task Get_Player_List_Filter_By_PlayerStatus_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new VipLevel
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new VipLevel
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "4ucmqplxkuyd",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "5692453796",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[0].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "4ucmqplwxqlv",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.InBlacklist,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 1000,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = players.Select(p => new PlayerIpRecord
            {
                PlayerCode = p.PlayerCode,
                Host = "www.google.com",
                Ip = "127.0.0.1",
                IpType = 1,
                RecordOn = p.RegisteredOn!.Value
            }).ToArray();

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                null,
                PlayerStatus.Enabled,
                null);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public async Task Get_Player_List_Filter_By_Register_Time_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "4ucmqplxkuyd",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "5692453796",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[0].Id,
                    RegisteredOn = new DateTimeOffset(2022, 02, 08, 11, 00, 00, TimeSpan.FromHours(8)),
                    UpdatedOn = DateTimeOffset.UtcNow
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "4ucmqplwxqlv",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = new DateTimeOffset(2022, 02, 09, 11, 00, 00, TimeSpan.FromHours(8)),
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = new PlayerIpRecord[]
            {
                new()
                {
                    PlayerCode ="4ucmqplxkuyd",
                    Host = "tw.yahoo.com",
                    Ip = "192.168.0.12",
                    IpType = 1,
                    RecordOn = new DateTimeOffset(2022, 02, 08, 11, 00, 00, TimeSpan.FromHours(8))
                },
                new()
                {
                    PlayerCode ="4ucmqplwxqlv",
                    Host = "www.google.com",
                    Ip = "192.168.0.1",
                    IpType = 1,
                    RecordOn = new DateTimeOffset(2022, 02, 09, 11, 00, 00, TimeSpan.FromHours(8))
                }
            };

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                new DateTimeOffset(2022, 02, 08, 10, 00, 00, TimeSpan.FromHours(8)),
                new DateTimeOffset(2022, 02, 08, 12, 00, 00, TimeSpan.FromHours(8)),
                null,
                null,
                null);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public async Task Get_Player_List_Filter_By_RegisteredHost_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "4ucmqplxkuyd",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    Status = PlayerStatus.RegisterUndone,
                    Type = PlayerType.RealPlayer
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "4ucmqplwxqlv",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = new PlayerIpRecord[]
            {
                new()
                {
                    PlayerCode ="4ucmqplwxqlv",
                    Host = "www.google.com",
                    Ip = "192.168.0.1",
                    IpType = 1,
                    RecordOn = DateTimeOffset.UtcNow
                }
            };

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                "www.google.com",
                null,
                null);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public async Task Get_Player_List_Filter_By_Username_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "black",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    Status = PlayerStatus.RegisterUndone,
                    Type = PlayerType.RealPlayer
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "white",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = new PlayerIpRecord[]
            {
                new()
                {
                    PlayerCode ="4ucmqplwxqlv",
                    Host = "www.google.com",
                    Ip = "192.168.0.1",
                    IpType = 1,
                    RecordOn = DateTimeOffset.UtcNow
                }
            };

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                "black",
                null,
                null,
                null,
                null,
                null);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public async Task Get_Player_List_Filter_By_VipLevel_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "4ucmqplxkuyd",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "5692453796",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[0].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "4ucmqplwxqlv",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 1000,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = players.Select(p => new PlayerIpRecord
            {
                PlayerCode = p.PlayerCode,
                Host = "www.google.com",
                Ip = "127.0.0.1",
                IpType = 1,
                RecordOn = p.RegisteredOn!.Value
            }).ToArray();

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                null,
                null,
                Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"));

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public async Task Get_Player_List_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var agentId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            var players = new Player[]
            {
                new()
                {
                    PlayerCode = "4ucmqplxkuyd",
                    Username = "4ucmqplxkuyd",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "5692453796",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[0].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                },
                new()
                {
                    PlayerCode = "4ucmqplwxqlv",
                    Username = "4ucmqplwxqlv",
                    BrandId = brandId,
                    DefaultWalletId = Guid.NewGuid(),
                    DefaultWalletCurrencyCode = "HKD",
                    AgentId = agentId,
                    AreaCode = "852",
                    Phone = "7461544109",
                    Password = Array.Empty<byte>(),
                    Salt = Array.Empty<byte>(),
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer,
                    VipId = vipLevels[1].Id,
                    RegisteredOn = DateTimeOffset.UtcNow,
                    UpdatedOn = DateTimeOffset.UtcNow
                }
            };

            var wallets = players.Select(p => new Wallet
            {
                Id = p.DefaultWalletId,
                PlayerCode = p.PlayerCode,
                CurrencyCode = p.DefaultWalletCurrencyCode,
                ExternalCurrencyCode = p.DefaultWalletCurrencyCode,
                Balance = 1000,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

            var ipRecords = players.Select(p => new PlayerIpRecord
            {
                PlayerCode = p.PlayerCode,
                Host = "www.google.com",
                Ip = "127.0.0.1",
                IpType = 1,
                RecordOn = p.RegisteredOn!.Value
            }).ToArray();

            repository.Players.AddRange(players);
            repository.Wallets.AddRange(wallets);
            repository.VipLevels.AddRange(vipLevels);
            repository.PlayerIpRecords.AddRange(ipRecords);

            await repository.SaveChangesAsync();

            var sut = new GetAgentPlayerInfosQueryService(repository);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                null,
                null,
                null);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(2, actual.Length);
        }

        [Test]
        public async Task Get_VipLevels_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.NewGuid();

            var vipLevels = new VipLevel[]
            {
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("77AC3B71-AB0D-471F-8F14-7AE1D3446043"),
                    Level = 1,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("B1611B0F-E1BA-4F51-9DC2-F6318BA37928"),
                    Level = 2,
                    CreatedOn = DateTimeOffset.Now
                },
                new()
                {
                    BrandId = brandId,
                    Id = Guid.Parse("2673009D-B8F0-4906-A43A-27012234BE06"),
                    Level = 3,
                    CreatedOn = DateTimeOffset.Now
                }
            };

            repository.VipLevels.AddRange(vipLevels);

            await repository.SaveChangesAsync();

            var sut = new GetPlayerVipLevelsQueryService(repository);

            var query = new GetPlayerVipLevelsQuery(brandId);

            var actual = await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(3, actual.Length);
        }
    }
}
