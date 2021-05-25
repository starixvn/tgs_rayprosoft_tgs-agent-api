using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Models.Data;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;

namespace TGS.UnitTests.Operations.Bets.Steps
{
    [Binding]
    [Scope(Tag = "ClientRoundStatusValidation")]
    public sealed class ClientRoundStatusValidation : SpecFlowBase
    {
        public ClientRoundStatusValidation(ScenarioContext context)
            : base(context)
        {
        }

        [Given(@"the round is ""(.*)""")]
        public void GivenTheRoundIs(RoundStatus status) =>
            Set(SR.RoundStatus, status);

        [Given(@"set HasTurnoverOnTieBet is ""(.*)""")]
        public void GivenSetHasTurnoverOnTieBetIs(bool hasTurnoverOnTieBet) =>
            Set(SR.HasTurnoverOnTieBet, hasTurnoverOnTieBet);

        [Given(@"a sequence of game actions")]
        public void GivenASequenceOfGameActions(Table table)
        {
            Set(SR.RoundId, Guid.NewGuid().ToString("N"));
            var actions = table.CreateList(ParseActionShortInfo());
            foreach (var action in actions)
            {
                AddAction(action.ID, _ => action.Amount, action.Type, action.BetType,
                    action.IsClosingRound, action.ReferencedId);
            }
        }

        [Given(@"with opposite bet patterns")]
        public void GivenWithOppositeBetPatterns(Table table)
        {
            var patterns = table.CreateList<OppositeBetPattern>(row =>
            {
                var betType = GetCell(row, "Bet Type");
                var group = int.TryParse(GetCell(row, "Group"), out var id) ? id : 0;

                return new(betType, group);
            }).ToList();

            Set(SR.OppositeBetPatterns, patterns);
        }

        [When(@"I get the client round status")]
        public void WhenIGetTheClientRoundStatus()
        {
            var betValuesCalculator =
                (IBetValuesCalculator)new TGS.Services.GameOperations.Calculators.BetValuesCalculator();

            var hasTurnoverOnTieBet = Get<bool>(SR.HasTurnoverOnTieBet);
            var oppositeBetPatterns = Get<List<OppositeBetPattern>>(SR.OppositeBetPatterns);

            var current = Actions.Last();
            var previousActions = Actions.Except(new[] {current}).ToArray();
            var betValuesResult =
                betValuesCalculator.Calculate(hasTurnoverOnTieBet, current, previousActions, oppositeBetPatterns);

            var gameActionWithSingleRound = Actions.Count == 1 &&
                Actions.Any(a => a.Type is GameActionType.FreeBet or GameActionType.WinJackpot);

            var isVoidRound = !gameActionWithSingleRound && Actions.Where(act => act.Type == GameActionType.PlaceBet)
                .All(act => act.IsCancelled);

            var roundStatus = Get<RoundStatus>(SR.RoundStatus);
            var manager = (IRoundStatusManager)new RoundStatusManager();
            var clientRoundStatus =
                manager.GetClientRoundStatus(roundStatus, betValuesResult, gameActionWithSingleRound, isVoidRound);

            Set(SR.ClientRoundStatus, clientRoundStatus);
        }

        [Then(@"the client round status should be ""(.*)""")]
        public void ThenTheClientRoundStatusShouldBe(ClientRoundStatus expectedRoundStatus)
        {
            var status = Get<ClientRoundStatus>(SR.ClientRoundStatus);
            status.Should().Be(expectedRoundStatus);
        }

        [After]
        public void RemoveHasTurnoverOnTieBet()
        {
            if (!HasTag("HasTurnoverOnTieBet"))
            {
                return;
            }

            Remove(SR.HasTurnoverOnTieBet);
        }

        [After]
        public void RemoveOppositeBetPatterns()
        {
            if (!HasTag("OppositeBets"))
            {
                return;
            }

            Remove(SR.OppositeBetPatterns);
        }

        #region Private Members

        private Func<TableRow, ActionShortInfo> ParseActionShortInfo() =>
            row =>
            {
                var isClosingRound = GetCell(row, "IsClosingRound");

                return new()
                {
                    ID = int.TryParse(GetCell(row, "ID"), out var id) ? id : NewActionId,
                    Type = AsType(row, "Action Type"),
                    Amount = decimal.TryParse(GetCell(row, "Amount"), out var amount) ? amount : 0,
                    ReferencedId = int.TryParse(GetCell(row, "Referencing Action ID"), out var refId) ? refId : 0,
                    BetType = GetCell(row, "Bet Type"),
                    IsClosingRound = !string.IsNullOrEmpty(isClosingRound) && isClosingRound == "YES"
                };
            };

        private string GetCell(TableRow row, string name) =>
            row.TryGetValue(name, out var value) ? value : string.Empty;

        private GameActionType AsType(TableRow row, string name)
        {
            var cellValue = (GetCell(row, name) ?? "").Replace(" ", "");
            if (string.IsNullOrEmpty(cellValue))
            {
                throw new($"Expecting GameActionType cell named {name}");
            }

            if (Enum.TryParse(cellValue, true, out GameActionType type))
            {
                return type;
            }

            throw new($"Unknown GameActionType {cellValue}");
        }

        private BetActionData CreateAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type,
            string betType, bool isClosingRound, int refId = 0)
        {
            BetActionData refAction = null;
            if (refId > 0 && !ActionsById.TryGetValue(refId, out refAction))
            {
                throw new($"Cannot find action with id {refId}");
            }

            return new()
            {
                Id = Guid.NewGuid(),
                ExternalId = id.ToString(),
                Amount = getAmount(refAction),
                ExternalRoundId = Get<string>(SR.RoundId),
                Type = type,
                ReferenceId = refAction?.Id,
                Reference = refAction,
                BetType = betType,
                IsClosingRound = isClosingRound
            };
        }

        private void AddAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type,
            string betType, bool isClosingRound, int refId = 0)
        {
            var action = CreateAction(id, getAmount, type, betType, isClosingRound, refId);
            AddAction(id, action);

            if (action.Type == GameActionType.CancelTransaction)
            {
                ActionsById[refId].IsCancelled = true;
            }
        }

        private void AddAction(int id, BetActionData action)
        {
            Actions.Add(action);
            ActionsById[id] = action;
        }

        private int _lastId = 999999;
        private int NewActionId => ActionsById.Count == 0 ? ++_lastId : ActionsById.Keys.Max() + 1;
        private List<BetActionData> Actions => GetOrCreate(SR.Actions, () => new List<BetActionData>());

        private Dictionary<int, BetActionData> ActionsById =>
            GetOrCreate(SR.ActionsById, () => new Dictionary<int, BetActionData>());

        private class ActionShortInfo
        {
            public int ID { get; set; }
            public GameActionType Type { get; set; }
            public decimal Amount { get; set; }
            public int ReferencedId { get; set; }
            public string BetType { get; set; }
            public bool IsClosingRound { get; set; }
        }

        #endregion
    }
}