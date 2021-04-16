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

Scenario: 03.a - Place desktop bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I place bet of $10 for game "vs50aladdin"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 03.b - Place mobile bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Mobile"
		And player having balance of $100
	When I place bet of $10 for game "vs50aladdin"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 03.c - Place bet with jackpot contribution
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I place bet with jackpot of $10  for game "vs50aladdin"
		| JackpotId | ContributionAmount |
		| Mega      | 10                 |
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And jackpot contribute data in game action is as set value

Scenario: 04 - Win bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And I place bet of $10 for game "vs50aladdin"
	When I win bet for $15
	Then I will receive result response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15

Scenario: 05 - Lose bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And I place bet of $10 for game "vs50aladdin"
	When I lose bet
	Then I will receive endRound response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0

Scenario: 06 - Cancel transaction
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And I place bet of $10 for game "vs50aladdin"
	When I cancel transaction
	Then I will receive refund response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10

Scenario: 07 - Win jackpot
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I win jackpot of $1000 for game "vs50aladdin"
	Then I will receive jackpot win response of no error and balance of $1100
		And player wallet database record will be for $1100
		And game action will be recorded in the database of type "WinJackpot" for $1000

@BonusWin
Scenario: 08.a -  Win Bonus in Bet and Result
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given bonus code is "BigBonus"
		And I play the bonus for game "vs50aladdin"
	When I win the bonus of $30
	Then I will receive result response of no error and balance of $130
		And player wallet database record will be for $130
		And game action will be recorded in the database of type "WinBet" for $30
		And bonus code in round detail is as set value

@BonusWin
Scenario: 08.b -  Bonus Win
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given bonus code is "BigBonus"
		And I finish all bonus of 10 rounds and get total win $100 for game "vs50aladdin"
		And player having balance of $200
	When bet win request is triggered after the bonus are finished
	Then I will receive bet win response of no error and balance of $200

@PromoWin
Scenario: 09 -  Promo Win
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given a campaign id is "C001" and type is "T"
	When I win the campaign of $100 
	Then I will receive promo win response of no error and balance of $200
		And player wallet database record will be for $200
		And game action will be recorded in the database of type "FreeBet" for $100
		And campaign data in round detail is as set value