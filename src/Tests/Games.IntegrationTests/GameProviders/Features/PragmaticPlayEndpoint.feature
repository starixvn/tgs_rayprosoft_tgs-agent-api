@GameIntegration
@PragmaticPlay
Feature: PragmaticPlay Endpoint
	Test PragmaticPlay Endpoint

Scenario: 01.a -  Get authenticate player details
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I call authorize endpoint
	Then I will get authorize response and player balance will be $125

#Scenario: 01.b - Get player details after player logout
#    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
#        And player having balance of $125
#        And player logout
#	When I call authorize endpoint
#    Then I will get error authorize response and no player balance

#Scenario: 03.a - Get player balance
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $125
#	When I request the player balance
#	Then player balance response will be for $125
#
#Scenario: 03.b - Get player balance with deauthorized player token
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $125
#		And deauthorized player tokens
#	When I request the player balance
#	Then player balance response will be for $125
#
#Scenario: 04 - Place desktop bet
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $100
#	When I place bet of $10 for game "vs25vegas"
#	Then I will receive response of no error and balance of $90
#		And player wallet database record will be for $90
#		And game action will be recorded in the database of type "PlaceBet" for $-10
#		And game platform will be recorded in the database of type "Desktop"
#
#Scenario: 05 - Win bet
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $100
#		And placed bet of $10 for game "vs25vegas"
#	When I win bet for $15
#	Then I will receive result response of no error and balance of $105
#		And player wallet database record will be for $105
#		And game action will be recorded in the database of type "WinBet" for $15
#
#Scenario: 06 - Lose bet
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $100
#		And placed bet of $10 for game "vs25vegas"
#	When I lose bet
#	Then I will receive endRound response of no error and balance of $90
#		And player wallet database record will be for $90
#		And game action will be recorded in the database of type "LoseBet" for $0
#
#Scenario: 07 - Cancel transaction
#	Given authorized player token
#		And get player data via system api
#		And player having balance of $100
#		And placed bet of $10 for game "vs25vegas"
#	When I cancel transaction
#	Then I will receive refund response of no error and balance of $100
#		And player wallet database record will be for $100
#		And game action will be recorded in the database of type "CancelTransaction" for $10