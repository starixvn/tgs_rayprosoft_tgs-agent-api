@GameIntegration
@SexyGaming

Feature: SexyGaming Endpoint
	Test SexyGaming Endpoint

Scenario: 01 - Get player balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125


Scenario: 02.a - Place desktop bet
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I place bet of $10 for game "MX-LIVE-001"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10


Scenario: 03.b - Can win bet without player login
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "MX-LIVE-001"
		And balance now being $90
	Given player logout
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And the round will be closed