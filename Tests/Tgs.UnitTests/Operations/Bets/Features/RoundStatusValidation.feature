@Unit
@RoundStatusValidation
Feature: RoundStatusValidation
	
	Given a sequence of game action 
	validate round status within different scenarios

Scenario: 01.a - If I place 1 bet, I win the bet, the round should be closed with AutoCloseRound = true
	Given AutoCloseRound set to "True"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID |
			| PlaceBet    | 1  |                       |
		 And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID | 
		| WinBet      | 1                     |
	Then round should be "Closed"

Scenario: 01.b - If I place 1 bet, then I win the bet, the round should be open with AutoCloseRound = true and RequestToCloseRound = NO
	Given AutoCloseRound set to "True"
	 And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
	 And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID | RequestToCloseRound |
		| WinBet      | 1                     | NO                  |
	Then round should be "Open"

Scenario: 01.c - If I place 1 bet, then I cancel the bet, the round should be closed with AutoCloseRound = true and RequestToCloseRound = YES
	Given AutoCloseRound set to "True"
	 And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
	 And the round is "Open"
	When I perform action
		| Action Type       | Referencing Action ID | RequestToCloseRound |
		| CancelTransaction | 1                     | YES                 |                 
	Then round should be "Closed"

Scenario: 01.d - If I place 1 bet, then I win the bet, the round should be open with AutoCloseRound = false
	Given AutoCloseRound set to "False"
	 And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
	 And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID | 
		| WinBet      | 1                     |
	Then round should be "Open"

Scenario: 01.e - If I place 1 bet, then I win the bet, round should be open with AutoCloseRound = false and RequestToCloseRound = YES
	Given AutoCloseRound set to "False"
	 And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
	 And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID | RequestToCloseRound |
		| WinBet      | 1                     | YES                 |
	Then round should be "Closed"

Scenario: 02.a - If I bet and settle that bet, then cancel the settlement with RequestToCloseRound = NO, the round should be reopened with AutoCloseRound = true
	Given AutoCloseRound set to "True"
	   And a sequence of game actions
		| Action Type       | ID | Referencing Action ID |
		| PlaceBet          | 1  |                       |
		| WinBet            | 2  | 1                     |
	   And the round is "Closed"
	When I perform action
		| Action Type       | Referencing Action ID | RequestToCloseRound |
		| CancelTransaction | 2                     | No                  |
	Then round should be "Open"

Scenario: 02.b - If I bet and settle that bet, then cancel the settlement, the round should be still closed with AutoCloseRound = true
	Given AutoCloseRound set to "True"
	   And a sequence of game actions
		| Action Type       | ID | Referencing Action ID |
		| PlaceBet          | 1  |                       |
		| WinBet            | 2  | 1                     |
	   And the round is "Closed"
	When I perform action
		| Action Type       | Referencing Action ID | 
		| CancelTransaction | 2                     | 
	Then round should be "Closed"

Scenario: 02.c - If I bet and settle that bet, then cancel the settlement with RequestToCloseRound = NO, the round should be reopened with AutoCloseRound = false
	Given AutoCloseRound set to "False"
	   And a sequence of game actions
		| Action Type | ID | Referencing Action ID | 
		| PlaceBet    | 1  |                       |                     
		| WinBet      | 2  | 1                     |
	   And the round is "Closed"
	When I perform action
		| Action Type       | Referencing Action ID | RequestToCloseRound |
		| CancelTransaction | 2                     | No                  |
	Then round should be "Open"

Scenario: 03.a - If I place multiple bets and settle the bets, the round should be closed with AutoCloseRound = true
	Given AutoCloseRound set to "True"
	   And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
		| PlaceBet    | 2  |                       |
		| WinBet      | 3  | 1                     |
	   And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID |
		| LoseBet     | 2                     |
	Then round should be "Closed"

Scenario: 03.b - If I place multiple bets and settle the bets with one free bet, the round should be closed with AutoCloseRound = true
	Given AutoCloseRound set to "True"
	   And a sequence of game actions
		| Action Type | ID | Referencing Action ID |
		| PlaceBet    | 1  |                       |
		| PlaceBet    | 2  |                       |
		| WinBet      | 3  | 1                     |
		| FreeBet     | 4  |                       |
	   And the round is "Open"
	When I perform action
		| Action Type | Referencing Action ID |
		| LoseBet     | 2                     |
	Then round should be "Closed"
	
Scenario: 04 - If I bet and settle multiple bets, then cancel the settlements, and resettle all the bets, the round should be closed with AutoCloseRound = true
	Given AutoCloseRound set to "True"
	   And a sequence of game actions
		| Action Type		| ID | Referencing Action ID | RequestToCloseRound |
		| PlaceBet			| 1  |                       |					   |
		| PlaceBet			| 2  |                       |					   |
		| LoseBet			| 3  | 1                     |					   |
		| LoseBet			| 4  | 2                     |					   |
		| CancelTransaction | 5  | 3                     | NO				   |
		| CancelTransaction | 6  | 4                     | NO				   |
	   And the round is "Open"
	When I perform action
		| Action Type       | Referencing Action ID | RequestToCloseRound |
		| WinBet			| 1                     |                     |
		| WinBet			| 2                     |                     |
	Then round should be "Closed"