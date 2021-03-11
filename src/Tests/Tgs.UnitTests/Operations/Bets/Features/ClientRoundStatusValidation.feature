@Unit
@ClientRoundStatusValidation
Feature: ClientRoundStatusValidation

	Given a sequence of game action 
	validate round status within different scenarios

Scenario: 01.a - If I place 1 bet, and the round is open, the client round status should be open
	Given the round is "Open"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount |
			| PlaceBet    | 1  |                       | -10    |
	When I get the client round status
	Then the client round status should be "Open"

Scenario: 01.b - If I place 1 bet and win the bet, and the round is open, the client round status should be open
	Given the round is "Open"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount |
			| PlaceBet    | 1  |                       | -10    |
			| WinBet      | 2  | 1                     | 10     |
	When I get the client round status
	Then the client round status should be "Open"

Scenario: 02.a - If I bet and win, and the round is closed, the client round status should be win
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| PlaceBet    | 1  |                       | -10    |                |
			| WinBet      | 2  | 1                     | 20     | YES            |
	When I get the client round status
	Then the client round status should be "Win"

Scenario: 02.b - If I bet and lose, and the round is closed, the client round status should be loss
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| PlaceBet    | 1  |                       | -10    |                |
			| LoseBet     | 2  | 1                     | 0      | YES            |
	When I get the client round status
	Then the client round status should be "Loss"

Scenario: 02.c - If I bet and the bet is tie, and the round is closed, the client round status should be draw
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| PlaceBet    | 1  |                       | -10    |                |
			| TieBet      | 2  | 1                     | 10     | YES            |
	When I get the client round status
	Then the client round status should be "Draw"

Scenario: 02.d - If I place 1 bet and the bet is cancelled, and the round is closed, the client round status should be void
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type       | ID | Referencing Action ID | Amount | IsClosingRound |
			| PlaceBet          | 1  |                       | -10    |                |
			| CancelTransaction | 2  | 1                     | 10     | YES            |
	When I get the client round status
	Then the client round status should be "Void"

Scenario: 02.e - If I bet, settled and win a free bet, and the round is closed, the client round status should be win
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| PlaceBet    | 1  |                       | -10    |                |
			| WinBet      | 2  | 1                     | 20     | YES            |
			| FreeBet     | 2  | 1                     | 10     | YES            |
	When I get the client round status
	Then the client round status should be "Win"

Scenario: 03.a - If I win a free bet in a single round, and the round is closed, the client round status should be win
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| FreeBet     | 1  |                       | 10     | YES            |
	When I get the client round status
	Then the client round status should be "Win"

Scenario: 03.b - If I lose a free bet in a single round, and the round is closed, the client round status should be loss
	Given the round is "Closed"
		And a sequence of game actions
			| Action Type | ID | Referencing Action ID | Amount | IsClosingRound |
			| FreeBet     | 1  |                       | 0      | YES            |
	When I get the client round status
	Then the client round status should be "Loss"