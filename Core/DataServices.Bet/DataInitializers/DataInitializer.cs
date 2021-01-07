using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.DataInitializers;
using TGS.Core.DataServices.Bet.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using static TGS.Core.Models.Enums.GameType;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Core.DataServices.Bet.DataInitializers
{
    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : BaseDataInitializer
    {
        private readonly GameProvider[] _gameProviders;
        private readonly Player[] _players;
        private readonly IBetRepository _repository;

        public DataInitializer(IBetRepository repository)
        {
            _repository = repository;

            var brands = new Brand[]
            {
                new(new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A"), "TGS",
                    new("B5A2C708-AB70-4316-9F4D-5D2AD0E77460"), "TGS",
                    new("66C5FEF5-217B-4929-9A08-72C2479A8F4E"), "TGS"),
                new(new("4E86719F-243F-46F6-B010-5C758B7285AF"), "GPI",
                    new("B5A2C708-AB70-4316-9F4D-5D2AD0E77460"), "TGS",
                    new("66C5FEF5-217B-4929-9A08-72C2479A8F4E"), "TGS")
            };

            _players = new Player[]
            {
                new("4ucmqplwxqlv", new("2EA6CDEF-094F-EB11-8D88-70B5E82B6B8D"), "CNY",
                    PlayerType.RealPlayer, brands[0]),
                new("4ucmqplx11cs", new("30A6CDEF-094F-EB11-8D88-70B5E82B6B8D"), "CNY",
                    PlayerType.RealPlayer, brands[0]),
                new("4ucmqplxhfoe", new("3AA6CDEF-094F-EB11-8D88-70B5E82B6B8D"), "CNY",
                    PlayerType.RealPlayer, brands[1]),
                new("4ucmqplxkuyd", new("3CA6CDEF-094F-EB11-8D88-70B5E82B6B8D"), "CNY",
                    PlayerType.RealPlayer, brands[1])
            };

            _gameProviders = new GameProvider[]
            {
                new(new("8D53C1F2-A718-4204-803F-5B33865175C5"), Consts.GameProvider.SBOBET, "SBOBET",
                    new("CC8AA6E0-5A7F-44BC-B294-5097C3518C37"),
                    new Game[]
                    {
                        new(100001, "Sportsbook", Sportsbook, PlatformType.Desktop),
                        new(100014, "Virtual Sports", VirtualSports, PlatformType.Desktop)
                    })
            };
        }

        protected override async Task SetInitialDataAsync()
        {
            if (_repository.Rounds.Any())
            {
                return;
            }

            var logBuilder = new StringBuilder();
            var timer = new Stopwatch();
            timer.Start();

            await LogSeedingTimeAsync(GenerateBetDataAsync, logBuilder);

            timer.Stop();

            logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            Console.WriteLine(logBuilder.ToString());
        }

        private async Task GenerateBetDataAsync()
        {
            var rand = new Random((int) (DateTime.UtcNow.Ticks % 1000000));
            var timestamp = DateTimeOffset.UtcNow.AddDays(-2);

            foreach (var p in _players)
            foreach (var gp in _gameProviders)
            foreach (var game in gp.Games)
            {
                var createdOn = timestamp = timestamp.AddSeconds(1);
                var closedOn = game.Type == Sportsbook || game.Type == VirtualSports
                    ? timestamp.AddDays(1)
                    : timestamp.AddSeconds(3);

                var round = CreateRound(p, gp, game, createdOn, closedOn);

                var externalGameActionGroupId = round.ExternalRoundId;
                var betAmount = ((decimal) rand.Next(5, 20)).Negate();
                var placeBetAction = CreateGameAction(p, betAmount, gp.GameProviderConfigurationId, round.Id,
                    PlaceBet, externalGameActionGroupId, createdOn, createdOn);
                await _repository.GameActions.AddAsync(placeBetAction);

                var v = rand.NextDouble();
                var resultActionType = v > 0.5 ? LoseBet : v > 0.2 && v < 0.5 ? WinBet : CancelTransaction;

                var resultAmount = resultActionType switch
                {
                    LoseBet           => 0,
                    CancelTransaction => betAmount.Negate(),
                    _                 => betAmount.Negate() * 2
                };

                decimal turnover = 0, winloss = 0;
                if (resultActionType != CancelTransaction)
                {
                    turnover = betAmount.Negate();
                    winloss = (betAmount + resultAmount).Negate();

                    round.WinLoss = winloss;
                }

                var resultAction = CreateGameAction(p, resultAmount, gp.GameProviderConfigurationId, round.Id,
                    resultActionType, externalGameActionGroupId, closedOn, closedOn, turnover, winloss,
                    placeBetAction.Id);
                await _repository.GameActions.AddAsync(resultAction);

                await _repository.Rounds.AddAsync(round);
            }

            await _repository.SaveChangesAsync();
        }

        private static Round CreateRound(Player player, GameProvider gameProvider, Game game,
            DateTimeOffset createdOn, DateTimeOffset closedOn)
        {
            var roundId = Guid.NewGuid();
            var brand = player.Brand;

            return new()
            {
                Id = roundId,
                ExternalRoundId = "EXT_" + roundId,
                GameId = game.Id,
                GameName = game.Name,
                GameType = game.Type,
                GameProviderId = gameProvider.Id,
                GameProviderCode = gameProvider.Code,
                GameProviderName = gameProvider.Name,
                GameProviderConfigurationId = gameProvider.GameProviderConfigurationId,
                WalletId = player.WalletId,
                PlayerCode = player.PlayerCode,
                PlayerType = player.Type,
                CurrencyCode = player.CurrencyCode,
                PlatformType = game.PlatformType,
                Status = RoundStatus.Closed,
                ResellerId = brand.ResellerId,
                ResellerCode = brand.ResellerCode,
                LicenseeId = brand.LicenseeId,
                LicenseeCode = brand.LicenseeCode,
                BrandId = brand.Id,
                BrandCode = brand.Code,
                CreatedOn = createdOn,
                ClosedOn = closedOn,
                ExternalCreatedOn = createdOn,
                ExternalClosedOn = closedOn
            };
        }

        private static GameAction CreateGameAction(Player player, decimal amount,
            Guid gameProviderConfigurationId, Guid roundId, GameActionType gameActionType,
            string externalGameActionGroupId, DateTimeOffset recordedOn, DateTimeOffset sentOn, decimal turnover = 0,
            decimal winloss = 0, Guid? refTransactionId = null, Guid? batchId = null)
        {
            var transactionId = Guid.NewGuid();
            var unsettledBets = gameActionType == PlaceBet ? amount : 0;

            return new()
            {
                Id = transactionId,
                ExternalTransactionId = $"EXT_{transactionId}",
                Type = gameActionType,
                RoundId = roundId,
                PlayerCode = player.PlayerCode,
                WalletId = player.WalletId,
                Turnover = turnover,
                WinLoss = winloss,
                UnsettledBets = unsettledBets,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameActionBatchId = batchId,
                RecordedOn = recordedOn,
                SentOn = sentOn,
                PlayerIp = "127.0.0.1",
                ExternalGameActionGroupId = externalGameActionGroupId,
                ExternalBetId = $"BET_{transactionId}",
                ReferencedGameActionId = refTransactionId,
                Amount = amount
            };
        }
    }
}