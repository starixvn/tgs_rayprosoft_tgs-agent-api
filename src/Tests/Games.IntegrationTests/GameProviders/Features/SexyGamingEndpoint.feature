@GameIntegration
@SexyGaming

Feature: SexyGaming Endpoint
	Test SexyGaming Endpoint

Scenario: 01 - Get player balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125