@GameIntegration
@Saba
Feature: Saba Endpoint
	Test Saba Endpoint

Scenario: 01 - Get player balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 02.a - Place desktop bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When bet is placed of $10 for game "1"
    Then I will receive place bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LockAmount" for $-10