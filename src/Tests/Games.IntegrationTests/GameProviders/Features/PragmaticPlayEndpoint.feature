@GameIntegration
@PragmaticPlay
Feature: PragmaticPlay Endpoint
	Test PragmaticPlay Endpoint

Scenario: 01.a -  Get authenticate player details
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I call authorize endpoint
	Then I will get authorize response and player balance will be $125

Scenario: 01.b - Get player details after player logout
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
	When I call authorize endpoint
    Then I will get error authorize response and no player balance

Scenario: 02.a - Get player balance
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 02.b - Get player balance with deauthorized player token
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
	When I request the player balance
	Then player balance response will be for $125

Scenario: 03 - Place desktop bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I place bet of $10 for game "vs50aladdin"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 04 - Win bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
		And I place bet of $10 for game "vs50aladdin"
	When I win bet for $15
	Then I will receive result response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15

Scenario: 05 - Lose bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
		And I place bet of $10 for game "vs50aladdin"
	When I lose bet
	Then I will receive endRound response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0

Scenario: 07 - Cancel transaction
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
		And I place bet of $10 for game "vs50aladdin"
	When I cancel transaction
	Then I will receive refund response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10