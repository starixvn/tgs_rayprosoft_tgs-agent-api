@Unit
@ReferencedActionValidation
Feature: Referenced Action Validation
	
	Validate each case in the table in /Readme/BusinessLogic/ReferencedActionsRules.md

Scenario: 1.1 - PlaceBet action can reference another PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "PlaceBet" action
	Then I will detect no errors

Scenario: 1.2 - FreeBet action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "FreeBet" action
	Then I will detect no errors

Scenario: 1.3 - WinBet action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "WinBet" action
	Then I will detect no errors

Scenario: 1.4 - LoseBet action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "LoseBet" action
	Then I will detect no errors

Scenario: 1.5 - TieBet action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "TieBet" action
	Then I will detect no errors

Scenario: 1.6 - CancelTransaction action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 1.7 - LockAmount action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "LockAmount" action
	Then I will detect no errors
	
Scenario: 1.8 - UnlockAmount action can reference PlaceBet action
	Given initial action "PlaceBet"
	When it is referenced by "UnlockAmount" action
	Then I will detect no errors

Scenario: 2.1 - PlaceBet action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "PlaceBet" action
	Then I will detect error containing "it is not allowed to have 'PlaceBet' action with a referenced action of type 'FreeBet'"

Scenario: 2.2 - FreeBet action can reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "FreeBet" action
	Then I will detect no errors

Scenario: 2.3 - WinBet action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "WinBet" action
	Then I will detect error containing "it is not allowed to have 'WinBet' action with a referenced action of type 'FreeBet'"

Scenario: 2.4 - LoseBet action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "it is not allowed to have 'LoseBet' action with a referenced action of type 'FreeBet'"

Scenario: 2.5 - TieBet action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "TieBet" action
	Then I will detect error containing "it is not allowed to have 'TieBet' action with a referenced action of type 'FreeBet'"
	
Scenario: 2.6 - CancelTransaction action can reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 2.7 - LockAmount action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "it is not allowed to have 'LockAmount' action with a referenced action of type 'FreeBet'"

Scenario: 2.8 - UnlockAmount action cannot reference FreeBet action
	Given initial action "FreeBet"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "it is not allowed to have 'UnlockAmount' action with a referenced action of type 'FreeBet'"

Scenario: 3.1 - PlaceBet action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "PlaceBet" action
	Then I will detect error containing "it is not allowed to have 'PlaceBet' action with a referenced action of type 'WinBet'"

Scenario: 3.2 - FreeBet action can reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "FreeBet" action
	Then I will detect no errors
	
Scenario: 3.3 - WinBet action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "WinBet" action
	Then I will detect error containing "it is not allowed to have 'WinBet' action with a referenced action of type 'WinBet'"
	
Scenario: 3.4 - LoseBet action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "it is not allowed to have 'LoseBet' action with a referenced action of type 'WinBet'"

Scenario: 3.5 - TieBet action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "TieBet" action
	Then I will detect error containing "it is not allowed to have 'TieBet' action with a referenced action of type 'WinBet'"

Scenario: 3.6 - CancelTransaction action can reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 3.7 - LockAmount action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "it is not allowed to have 'LockAmount' action with a referenced action of type 'WinBet'"

Scenario: 3.8 - UnlockAmount action cannot reference WinBet action
	Given initial action "WinBet"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "it is not allowed to have 'UnlockAmount' action with a referenced action of type 'WinBet'"

Scenario: 4.1 - PlaceBet action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "PlaceBet" action
	Then I will detect error containing "it is not allowed to have 'PlaceBet' action with a referenced action of type 'LoseBet'"

Scenario: 4.2 - FreeBet action can reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "FreeBet" action
	Then I will detect no errors
	
Scenario: 4.3 - WinBet action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "WinBet" action
	Then I will detect error containing "it is not allowed to have 'WinBet' action with a referenced action of type 'LoseBet'"
	
Scenario: 4.4 - LoseBet action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "it is not allowed to have 'LoseBet' action with a referenced action of type 'LoseBet'"

Scenario: 4.5 - TieBet action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "TieBet" action
	Then I will detect error containing "it is not allowed to have 'TieBet' action with a referenced action of type 'LoseBet'"
	
Scenario: 4.6 - CancelTransaction action can reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 4.7 - LockAmount action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "it is not allowed to have 'LockAmount' action with a referenced action of type 'LoseBet'"

Scenario: 4.8 - UnlockAmount action cannot reference LoseBet action
	Given initial action "LoseBet"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "it is not allowed to have 'UnlockAmount' action with a referenced action of type 'LoseBet'"

Scenario: 5.1 - PlaceBet action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "PlaceBet" action
	Then I will detect error containing "it is not allowed to have 'PlaceBet' action with a referenced action of type 'TieBet'"

Scenario: 5.2 - FreeBet action can reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "FreeBet" action
	Then I will detect no errors
	
Scenario: 5.3 - WinBet action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "WinBet" action
	Then I will detect error containing "it is not allowed to have 'WinBet' action with a referenced action of type 'TieBet'"
	
Scenario: 5.4 - LoseBet action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "it is not allowed to have 'LoseBet' action with a referenced action of type 'TieBet'"

Scenario: 5.5 - TieBet action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "TieBet" action
	Then I will detect error containing "it is not allowed to have 'TieBet' action with a referenced action of type 'TieBet'"

Scenario: 5.6 - CancelTransaction action can reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 5.7 - LockAmount action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "it is not allowed to have 'LockAmount' action with a referenced action of type 'TieBet'"

Scenario: 5.8 - UnlockAmount action cannot reference TieBet action
	Given initial action "TieBet"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "it is not allowed to have 'UnlockAmount' action with a referenced action of type 'TieBet'"

Scenario: 6.1 - PlaceBet action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "PlaceBet" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"

Scenario: 6.2 - FreeBet action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "FreeBet" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"

Scenario: 6.3 - WinBet action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "Winbet" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"
	
Scenario: 6.4 - LoseBet action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"
	
Scenario: 6.5 - TieBet action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "TieBet" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"
	
Scenario: 6.6 - CancelTransaction action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "CancelTransaction" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"

Scenario: 6.7 - LockAmount action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"

Scenario: 6.8 - UnlockAmount action cannot reference CancelTransaction action
	Given initial action "CancelTransaction"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "you cannot perform any action on action of type 'CancelTransaction' for reference"
	
Scenario: 7.1 - PlaceBet action can reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "PlaceBet" action
	Then I will detect no errors

Scenario: 7.2 - FreeBet action cannot reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "FreeBet" action
	Then I will detect error containing "it is not allowed to have 'FreeBet' action with a referenced action of type 'LockAmount'"
	
Scenario: 7.3 - WinBet action cannot reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "WinBet" action
	Then I will detect error containing "it is not allowed to have 'WinBet' action with a referenced action of type 'LockAmount'"
	
Scenario: 7.4 - LoseBet action cannot reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "LoseBet" action
	Then I will detect error containing "it is not allowed to have 'LoseBet' action with a referenced action of type 'LockAmount'"

Scenario: 7.5 - TieBet action cannot reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "TieBet" action
	Then I will detect error containing "it is not allowed to have 'TieBet' action with a referenced action of type 'LockAmount'"

Scenario: 7.6 - CancelTransaction action can reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 7.7 - LockAmount action can reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "LockAmount" action
	Then I will detect no errors

Scenario: 7.8 - UnlockAmount action can reference LockAmount action
	Given initial action "LockAmount"
	When it is referenced by "UnlockAmount" action
	Then I will detect no errors

Scenario: 8.1 - PlaceBet action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "PlaceBet" action
	Then I will detect no errors

Scenario: 8.2 - FreeBet action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "FreeBet" action
	Then I will detect no errors
	
Scenario: 8.3 - WinBet action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "WinBet" action
	Then I will detect no errors
	
Scenario: 8.4 - LoseBet action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "LoseBet" action
	Then I will detect no errors

Scenario: 8.5 - TieBet action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "TieBet" action
	Then I will detect no errors

Scenario: 8.6 - CancelTransaction action can reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "CancelTransaction" action
	Then I will detect no errors
	
Scenario: 8.7 - LockAmount action cannot reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "LockAmount" action
	Then I will detect error containing "it is not allowed to have 'LockAmount' action with a referenced action of type 'UnlockAmount'"

Scenario: 8.8 - UnlockAmount action cannot reference UnlockAmount action
	Given initial action "UnlockAmount"
	When it is referenced by "UnlockAmount" action
	Then I will detect error containing "it is not allowed to have 'UnlockAmount' action with a referenced action of type 'UnlockAmount'"
