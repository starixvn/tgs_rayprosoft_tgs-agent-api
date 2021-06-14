@GameIntegration
@JDB

Feature: JDBEndpoint
	Test JDB Endpoint

Scenario: 01 - Get player balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 02.a - Place desktop bet and win
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	When I place bet of $10 for game "14023" and win $20
	Then I will receive bet response of no error and balance of $110
	And player wallet database record will be for $110
	And game action will be recorded in the database of type "PlaceBet" for $-10
	And game action will be recorded in the database of type "WinBet" for $20
	And the round will be closed

Scenario: 02.b - Place mobile bet and win
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Mobile"
	And player having balance of $100
	And set game type as "Slot"
	When I place bet of $10 for game "14023" and win $20
	Then I will receive bet response of no error and balance of $110
	And player wallet database record will be for $110
	And game action will be recorded in the database of type "PlaceBet" for $-10
	And game action will be recorded in the database of type "WinBet" for $20
	And the round will be closed
	
Scenario: 02.c - Place desktop bet and lose
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	When I place bet of $10 for game "14023" and lose
	Then I will receive bet response of no error and balance of $90
	And player wallet database record will be for $90
	And game action will be recorded in the database of type "PlaceBet" for $-10
	And game action will be recorded in the database of type "LoseBet" for $0
	And the round will be closed
	
Scenario: 02.d - Place mobile bet and lose
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	When I place bet of $10 for game "14023" and lose
	Then I will receive bet response of no error and balance of $90
	And player wallet database record will be for $90
	And game action will be recorded in the database of type "PlaceBet" for $-10
	And game action will be recorded in the database of type "LoseBet" for $0
	And the round will be closed

@ChangeGameStatus
Scenario: 02.e - Can't place desktop bet and settle if game is under maintenance
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	And game "14023" has status set to "Maintenance"
	When I place bet of $10 for game "14023" and win $20
	Then I will receive bet response of error "SystemIsMaintained"
	
@ChangeBrandStatus
Scenario: 02.f - Can't place desktop bet and settle if brand is under maintenance
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	And brand has status set to "Maintenance"
	When I place bet of $10 for game "14023" and win $20
	Then I will receive bet response of error "SystemIsMaintained"
	
Scenario: 02.g - Place desktop bet and settle with insufficient balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $5
	And set game type as "Slot"
	When I place bet of $10 for game "14023" and win $20
	Then I will receive bet response of error "BalanceNotEnough"
	
Scenario: 02.h - Place desktop bet and settle with duplicate transaction ID
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	And I place bet of $10 for game "14023" and win $20
	When  I resend the same bet
	Then I will receive bet response of no error and balance of $110
	And player wallet database record will be for $110
	And game action will be recorded in the database of type "PlaceBet" for $-10
	And game action will be recorded in the database of type "WinBet" for $20
	And the round will be closed
	
Scenario: 03.a - Void round
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	And set game type as "Slot"
	And I place bet of $10 for game "14023" and win $20
	When I void round
	Then I will receive bet response of no error and balance of $100
	And all cancel bet related database records will be created and the balance of each user will be matched as original
	
Scenario: 03.b - Void not exist round
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
	And player having balance of $100
	When I void round
	Then I will receive bet response of error "DataDoesNotExist"