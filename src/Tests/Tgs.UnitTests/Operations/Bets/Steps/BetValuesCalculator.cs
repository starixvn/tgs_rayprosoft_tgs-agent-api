using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Models.Data;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;

namespace TGS.UnitTests.Operations.Bets.Steps
{
    [Binding]
    [Scope(Tag = "BetValuesCalculator")]
    public sealed class BetValuesCalculator : SpecFlowBase
    {
        public BetValuesCalculator(ScenarioContext context)
            : base(context)
        {
        }

        [Given(@"initial balance of \$(.*)")]
        public void GivenInitialBalanceOf(decimal amount) =>
            Set(SR.UserBalance, amount);

        [Given(@"game with no turnover on tie bet")]
        public void GivenGameWithNoTurnoverOnTieBet() =>
            Set(SR.TurnoverOnTieBet, false);

        [Given(@"game with turnover on tie bet")]
        public void GivenGameWithTurnoverOnTieBet() =>
            Set(SR.TurnoverOnTieBet, true);

        [Given(@"Opposite Bet Patterns")]
        public void GivenOppositeBetPatterns(Table table)
        {
            var patterns = table.CreateList(r =>
            {
                if (!int.TryParse(r[0], out var groupId))
                {
                    groupId = 0;
                }

                return new OppositeBetPattern(r[1], groupId);
            }).ToArray();

            Set(SR.OppositeBetPatterns, patterns);
        }


        [When(@"I perform the following actions I will see the expected results")]
        public void WhenIPerformTheFollowingActionsIWillSeeTheExpectedResults(Table table)
        {
            var rows = table.CreateList(CreateBetValuesCalculatorRowInstance);
            var dupRowId = rows.GroupBy(r => r.Id).FirstOrDefault(g => g.Count() > 1);
            if (dupRowId != null)
            {
                throw new ArgumentException("Test definition error: Duplicate row ID=" +
                    dupRowId.Key.ToString().TrimStart('0', '-'));
            }

            var hasTurnoverOnTieBet = Get(SR.TurnoverOnTieBet, true);

            var actionById = rows
                .Select((bvcr, _) => new BetActionData
                {
                    Id = bvcr.Id,
                    ReferenceId = bvcr.RefId,
                    Amount = bvcr.Amount,
                    Type = bvcr.ActionType,
                    IsClosingRound = bvcr.RoundClosed.HasValue && bvcr.RoundClosed.Value,
                    IsExplicitlyClosingRound = bvcr.RoundClosed.HasValue && bvcr.RoundClosed.Value,
                    IsExplicitlyNotClosingRound = bvcr.RoundClosed.HasValue && !bvcr.RoundClosed.Value,
                    GameProviderCode = "Test",
                    GameName = "Test",
                    BetType = bvcr.BetType,
                    JackpotContribution = bvcr.JackpotContributionAmount.HasValue
                        ? new JackpotContribution
                        {
                            JackpotId = Guid.NewGuid().ToString(),
                            TotalAmount = bvcr.JackpotContributionAmount.Value
                        }
                        : null
                }).ToDictionary(a => a.Id, a => a);

            foreach (var a in actionById.Values)
            {
                if (a.ReferenceId != null && actionById.TryGetValue(a.ReferenceId.Value, out var val))
                {
                    a.Reference = val;
                }
            }

            var previousInput = new List<BetActionData>();
            var previousOutput = new List<BetValuesResult>();
            var calculator = (IBetValuesCalculator)new DefaultBetValuesCalculator();

            var oppositeBetPatterns = HasTag("OppositeBets")
                ? Get<IList<OppositeBetPattern>>(SR.OppositeBetPatterns)
                : null;

            foreach (var current in rows)
            {
                var action = actionById[current.Id];
                if (current.Type != BetValuesCalculatorRowName.Totals)
                {
                    if (action.Type == GameActionType.CancelTransaction)
                    {
                        action.Reference.IsCancelled = true;
                        if (action.Reference.Type == GameActionType.CancelTransaction)
                        {
                            action.Reference.Reference.IsCancelled = false;
                        }
                    }

                    // Act
                    var result =
                        calculator.Calculate(
                            hasTurnoverOnTieBet,
                            action,
                            previousInput,
                            oppositeBetPatterns);

                    previousOutput.Add(result);

                    // Assert
                    ValidateBetResult(current, result);

                    action.WinLoss = result.WinLoss;
                    action.Turnover = result.Turnover;
                    action.UnsettledBets = result.UnsettledBets;
                }
                else
                {
                    // Assert
                    ValidateTotals(previousInput, previousOutput, current);
                }

                previousInput.Add(action);
            }

            Set(SR.BetValuesResults, previousOutput);
        }

        [Then(@"the final bet amount is (.*) and valid bet amount is (.*) and settle amount is (.*)")]
        public void ThenTheFinalBetAmountIsAndValidBetAmountIsAndSettleAmountIs(decimal betAmount,
            decimal validBetAmount, decimal settleAmount)
        {
            var results = Get<List<BetValuesResult>>(SR.BetValuesResults);
            var last = results.Last();

            last.BetAmount.Should().Be(betAmount);
            last.ValidBetAmount.Should().Be(validBetAmount);
            last.SettleAmount.Should().Be(settleAmount);
        }


        #region Private Methods and Properties

        private static BetValuesCalculatorRow CreateBetValuesCalculatorRowInstance(TableRow row) =>
            new()
            {
                Id = ParseGuid(row, "Id") ?? Guid.Empty,
                RefId = ParseGuid(row, "Ref Id"),
                Type = ParseRowName(row, "Type"),
                Amount = ParseDecimal(row, "Amount"),
                PostBalance = ParseDecimal(row, "Post Balance"),
                Turnover = ParseDecimal(row, "Turnover"),
                TieBetAdjustment = ParseDecimal(row, "TieBetAdjustment"),
                WinLoss = ParseDecimal(row, "WinLoss"),
                UnsettledBets = ParseDecimal(row, "Unsettled Bets"),
                RoundClosed = ParseNullableBool(row, "Is Closing Round"),
                BetType = row["Bet Type"],
                JackpotContributionAmount = ParseNullableDecimal(row, "JackpotContributionAmount")
            };

        private static Guid? ParseGuid(TableRow row, string name) =>
            Guid.TryParse((row[name] ?? string.Empty).PadLeft(32, '0'), out var id) ? id : null;

        private static decimal ParseDecimal(TableRow row, string name) =>
            decimal.Parse(row[name].Replace("$", string.Empty));

        private static decimal? ParseNullableDecimal(TableRow row, string name)
        {
            var value = row[name];
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return decimal.Parse(value.Replace("$", string.Empty));
        }

        private static BetValuesCalculatorRowName ParseRowName(TableRow row, string name) =>
            (BetValuesCalculatorRowName)Enum.Parse(typeof(BetValuesCalculatorRowName), row[name], true);

        private static bool? ParseNullableBool(TableRow row, string name)
        {
            var s = row[name].ToLowerInvariant().Trim();
            return s switch
            {
                "yes"   => true,
                "true"  => true,
                "no"    => false,
                "false" => false,
                _       => null
            };
        }

        private static void ValidateTotals(IList<BetActionData> previousInput, IList<BetValuesResult> previousOutput,
            BetValuesCalculatorRow current)
        {
            var amount = previousInput.Sum(e => e.Amount);

            amount.Should()
                .Be(current.Amount, "Amount");

            previousOutput
                .Sum(e => e.Turnover)
                .Should()
                .Be(current.Turnover, "Turnover");

            previousOutput
                .Sum(e => e.WinLoss)
                .Should()
                .Be(current.WinLoss, "WinLoss");

            var tieBetAdjustment = previousOutput
                .Sum(e => e.TieBetAdjustment);

            tieBetAdjustment.Should()
                .Be(current.TieBetAdjustment, "TieBetAdjustment");

            previousOutput
                .Sum(e => e.UnsettledBets)
                .Should()
                .Be(current.UnsettledBets, "Unsettled Bets");
        }

        private static void ValidateBetResult(BetValuesCalculatorRow current, BetValuesResult result)
        {
            var idAsInt = current.Id.ToString("N").TrimStart('0');

            result.Turnover.Should().Be(current.Turnover, $" that is Turnover [for row:{current.Type} ID={idAsInt}]");
            result.WinLoss.Should().Be(current.WinLoss, $" that is WinLoss [for row:{current.Type} ID={idAsInt}]");
            result.UnsettledBets.Should().Be(current.UnsettledBets,
                $" that is Unsettled Bets [for row:{current.Type} ID={idAsInt}]");
            result.TieBetAdjustment.Should().Be(current.TieBetAdjustment,
                $" that is TieBetAdjustment Bets [for row:{current.Type} ID={idAsInt}]");
        }

        #endregion Private Methods and Properties
    }

    internal enum BetValuesCalculatorRowName
    {
        PlaceBet = GameActionType.PlaceBet,
        WinBet = GameActionType.WinBet,
        CancelTransaction = GameActionType.CancelTransaction,
        LoseBet = GameActionType.LoseBet,
        FreeBet = GameActionType.FreeBet,
        TieBet = GameActionType.TieBet,
        WinJackpot = GameActionType.WinJackpot,
        Totals = short.MaxValue
    }

    internal class BetValuesCalculatorRow
    {
        public Guid Id { get; set; }
        public Guid? RefId { get; set; }
        public BetValuesCalculatorRowName Type { get; set; }
        public GameActionType ActionType => (GameActionType)Type;
        public decimal Amount { get; set; }
        public decimal PostBalance { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLoss { get; set; }
        public decimal TieBetAdjustment { get; set; }
        public decimal UnsettledBets { get; set; }
        public string BetType { get; set; }
        public bool? RoundClosed { get; set; }
        public decimal? JackpotContributionAmount { get; set; }
    }
}