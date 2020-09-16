using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.GameOperations.Validators;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;

namespace Tgs.UnitTests.Operations.Bets.Steps
{
    [Binding, Scope(Tag = "ReferencedActionValidation")]
    public sealed class ReferencedActionValidation : SpecFlowUnitTestBase
    {
        public ReferencedActionValidation(ScenarioContext context)
            : base(context)
        {
        }

        [Given(@"initial action ""(.*)""")]
        public void GivenInitialAction(GameActionType actionType)
        {
            var table = new Table("Action Type", "ID");
            table.AddRow(actionType.ToString(), "1");
            AddActions(table);
        }

        [When(@"it is referenced by ""(.*)"" action")]
        public void WhenItIsReferencedByAction(GameActionType actionType)
        {
            var table = new Table("Action Type", "Referencing Action ID");
            table.AddRow(actionType.ToString(), "1");
            ValidatAction(table);
        }

        [Then(@"I will detect no errors")]
        public void ThenIWillDetectNoErrors()
        {
            var error = Get<Exception>(SR.ErrorResponse);
            error.Should().BeNull();
        }

        [Then(@"I will detect error containing ""(.*)""")]
        public void ThenIWillDetectErrorContaining(string errorMessage)
        {
            var error = Get<Exception>(SR.ErrorResponse);
            error.Should().NotBeNull("Expected to find error but no error was detected");
            error.Message
                .ToLowerInvariant()
                .Should()
                .Contain(errorMessage.ToLowerInvariant());
        }

        #region Private Methods and Properties

        private void AddActions(Table table)
        {
            Set(SR.RoundId, Guid.NewGuid().ToString("N"));
            var actions = table.CreateList(ParseActionShortInfo());
            foreach (var action in actions)
            {
                AddAction(action.ID, r => action.Amount, action.Type, action.IsClosingRound, action.ReferencedId);
            }
        }

        private Func<TableRow, ActionShortInfo> ParseActionShortInfo()
        {
            return row => new ActionShortInfo
            {
                ID = int.TryParse(GetCell(row, "ID"), out var id) ? id : NewActionId,
                Type = AsType(row, "Action Type"),
                Amount = decimal.TryParse(GetCell(row, "Amount"), out var amount) ? amount : 0,
                ReferencedId = int.TryParse(GetCell(row, "Referencing Action ID"), out var refId) ? refId : 0,
                IsClosingRound = GetCell(row, "Closeing Round?") == "YES"
            };
        }

        private string GetCell(TableRow row, string name)
        {
            return row.ContainsKey(name) ? row[name] : "";
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

        private BetActionData CreateAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type, bool isClosingRound, int refId = 0)
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
                IsClosingRound = isClosingRound,
                IsExplicitlyClosingRound = isClosingRound == true,
                IsExplicitlyNotClosingRound = isClosingRound == false,
                Type = type,
                ReferenceId = refAction?.Id,
                Reference = refAction
            };
        }

        private void AddAction(int id, BetActionData action)
        {
            Actions.Add(action);
            ActionsById[id] = action;
        }

        private void AddAction(int id, Func<BetActionData, decimal> getAmount, GameActionType type, bool isClosingRound, int refId = 0)
        {
            var action = CreateAction(id, getAmount, type, isClosingRound, refId);
            AddAction(id, action);
        }

        private void ValidatAction(Table table)
        {
            var asi = table.CreateList(ParseActionShortInfo()).FirstOrDefault();

            if (asi == null)
            {
                throw new Exception("Expecting action definition");
            }

            var getAmount = asi.Type == GameActionType.CancelTransaction
                        ? (r => r.Amount)
                        : new Func<BetActionData, decimal>(r => asi.Amount);

            var action = CreateAction(NewActionId, getAmount, asi.Type, asi.IsClosingRound, asi.ReferencedId);
            Validate(action);
        }

        private void Validate(BetActionData action)
        {
            IActionWithinRoundValidator validator = new ActionWithinRoundValidator();
            var result = validator.FindError(action, Actions);
            Set(SR.ErrorResponse, result);
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
            public bool IsClosingRound { get; set; }
        }

        #endregion Private Methods and Properties
    }
}