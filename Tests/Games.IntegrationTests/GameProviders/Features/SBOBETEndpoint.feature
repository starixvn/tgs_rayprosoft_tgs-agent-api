@GameIntegration
@SBOBET

Feature: SBOBET Endpoint
	Test SBOBET Endpoint

Scenario: 01 - Get player balance
	Given player "TP01_cny_en-us" of licensee "TGS" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125