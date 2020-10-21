using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Services.GameOperations.Models.Data;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;

namespace Tgs.UnitTests.Operations.Bets.Steps
{
    [Binding, Scope(Tag = "RoundStatusValidation")]
    public sealed class RoundStatusValidation : SpecFlowBase
    {
        public RoundStatusValidation(ScenarioContext context)
            : base(context)
        {
        }

        [Given(@"AutoCloseRound set to ""(.*)""")]
        public void GivenAutoCloseRoundSetTo(bool autoCloseRound)
        {
            Set(SR.AutoCloseRound, autoCloseRound);
        }

        [Given(@"a sequence of game actions")]
        public void GivenASequenceOfGameActions(Table table)
        {
            Set(SR.RoundId, Guid.NewGuid().ToString("N"));
            var actions = table.CreateList(ParseActionShortInfo());
            foreach (var action in actions)
            {
                AddAction(action.ID, r => action.Amount, action.Type, action.ReferencedId);
            }
        }

        [Given(@"the round is ""(.*)""")]
        public void GivenTheRoundIs(RoundStatus status)
        {
            Set(SR.RoundStatus, status);
        }

        [When(@"I perform action")]
        public void WhenIPerformAction(Table table)
        {
            var asi = table.CreateList(ParseActionShortInfo()).FirstOrDefault();

            if (asi == null)
            {
                throw new Exception("Expecting action definition");
            }

            var actions = table.CreateList(ParseActionShortInfo());
            var actionList =
                actions.Select(
                    action =>
                    {
                        Func<BetActionData, decimal> getAmount = action.Type == GameActionType.CancelTransaction
                                                        ? new Func<BetActionData, decimal>(r => r.Amount)
                                                        : (r => action.Amount);

                        return CreateAction(NewActionId, getAmount, action.Type, action.ReferencedId);
                    })
                    .ToList();

            Validate(actionList, asi.RequestToCloseRound);
        }

        [Then(@"round should be ""(.*)""")]
        public void ThenRoundShouldBe(RoundStatus expectedRoundStatus)
        {
            var status = Get<RoundStatus>(SR.RoundClosed);
            status.Should().Be(expectedRoundStatus);
        }

        #region Private Methods and Properties

        private void Validate(List<BetActionData> actions, bool? requestToCloseRound)
        {
            var autoCloseRound = Get<bool>(SR.AutoCloseRound);
            var previousRoundStatus = Get<RoundStatus>(SR.RoundStatus);
            var roundId = Guid.Parse(Get<string>(SR.RoundId));

            IRoundStatusManager manager = new RoundStatusManager();
            RoundStatus? status = null;

            foreach (var action in actions)
            {
                var validActions = GetValidPreviousActions(Actions);
                status = manager.GetStatus(new RoundStatusData
                {
                    RoundId = roundId,
                    AutoCloseRound = autoCloseRound,
                    RequestToCloseRound = requestToCloseRound,
                    OldStatus = previousRoundStatus,
                    GameActionType = action.Type,
                    ReferencedGameAction = action.Reference,
                    PreviousActions = Actions,
                    ValidPreviousActions = validActions
                });

                Actions.Add(action);
            }

            Set(SR.RoundClosed, status);
        }

        private static IList<BetActionData> GetValidPreviousActions(IList<BetActionData> previousActions)
        {
            var allCancelledIdsInTheRound = previousActions
                .Where(r => r.Type == GameActionType.CancelTransaction && r.Reference != null)
                .Select(r => r.Reference.Id)
                .ToHashSet();

            return previousActions
                .Where(r => !allCancelledIdsInTheRound.Contains(r.Id))
                .ToList();
        }

        private Func<TableRow, ActionShortInfo> ParseActionShortInfo()
        {
            return row =>
            {
                var requestToCloseRound = GetCell(row, "RequestToCloseRound");
                return new ActionShortInfo
                {
                    ID = int.TryParse(GetCell(row, "ID"), out var id) ? id : NewActionId,
                    Type = AsType(row, "Action Type"),
                    Amount = decimal.TryParse(GetCell(row, "Amount"), out var amount) ? amount : 0,
                    ReferencedId = int.TryParse(GetCell(row, "Referencing Action ID"), out var refId) ? refId : 0,
                    RequestToCloseRound = !string.IsNullOrEmpty(requestToCloseRound)
                                            ? requestToCloseRound == "YES"
                                            : new bool?()
                };
            };
        }

        private string GetCell(TableRow row, string name)
        {
            return row.TryGetValue(name, out var value) ? value : string.Empty;
        }

        private GameActionType AsType(TableRow row, string name)
        {
            var cellValue = (GetCell(row, name) ?? "").Replace(" ", "");
            if (string.IsNullOrEmpty(cellValue))
            {
                throw new Exception($"Expecting GameActionType cell named {name}");
            }

            if (Enum.TryParse(cellValue, true, out GameActionType type))
            {
                return type;
            }

            throw new Exception($"Unknown GameActionType {cellValue}");
        }

        private BetActionData CreateAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type, int refId = 0)
        {
            BetActionData refAction = null;
            if (refId > 0 && !ActionsById.TryGetValue(refId, out refAction))
            {
                throw new Exception($"Cannot find action with id {refId}");
            }

            return new BetActionData
            {
                Id = Guid.NewGuid(),
                ExternalId = id.ToString(),
                Amount = getAmount(refAction),
                ExternalRoundId = Get<string>(SR.RoundId),
                Type = type,
                ReferenceId = refAction?.Id,
                Reference = refAction
            };
        }

        private void AddAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type, int refId = 0)
        {
            var action = CreateAction(id, getAmount, type, refId);
            AddAction(id, action);
        }

        private void AddAction(int id, BetActionData action)
        {
            Actions.Add(action);
            ActionsById[id] = action;
        }

        private int _lastId = 999999;
        private int NewActionId => ActionsById.Count == 0 ? ++_lastId : ActionsById.Keys.Max() + 1;
        private List<BetActionData> Actions => GetOrCreate(SR.Actions, () => new List<BetActionData>());
        private Dictionary<int, BetActionData> ActionsById => GetOrCreate(SR.ActionsById, () => new Dictionary<int, BetActionData>());

        private class ActionShortInfo
        {
            public int ID { get; set; }
            public GameActionType Type { get; set; }
            public decimal Amount { get; set; }
            public int ReferencedId { get; set; }
            public bool? RequestToCloseRound { get; set; }
        }

        #endregion Private Methods and Properties
    }
}