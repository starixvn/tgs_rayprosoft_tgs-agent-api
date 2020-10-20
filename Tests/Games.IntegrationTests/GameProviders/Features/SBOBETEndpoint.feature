@GameIntegration
@SBOBET

Feature: SBOBET Endpoint
	Test SBOBET Endpoint

Scenario: 01.a - Get player balance
	Given player "TP01_cny_en-us" of licensee "TGS" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 01.b - Get player balance without login
	Given player "TP01_cny_en-us" of licensee "TGS" without login
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 02.a - Place desktop bet
	Given player "TP01_cny_en-us" of licensee "TGS" login via "Desktop"
		And player having balance of $100
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

Scenario: 02.b - Place mobile bet
	Given player "TP01_cny_en-us" of licensee "TGS" login via "Mobile"
		And player having balance of $100
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Mobile"
	