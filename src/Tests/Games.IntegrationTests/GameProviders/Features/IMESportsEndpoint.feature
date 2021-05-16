@GameIntegration
@IMESports

Feature: IMESportsEndpoint
	Test IM ESports Endpoint
	
Scenario: 01.a - Get player balance
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125
	
Scenario: 01.b - Get player balance without login
	Given player "TP01cny" of brand "GPI" without login
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125
	
Scenario: 02.a - Place desktop bet
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		
Scenario: 02.b - Place mobile bet
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Mobile"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		
Scenario: 02.c - Player cannot place bet without login
	Given player "TP01cny" of brand "GPI" without login
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports"
	Then I will receive bet response of error "InvalidToken"
	
Scenario: 02.d - Player cannot place bet with unknown game
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "1_0"
	When I place bet of $-10 for game "1_0"
	Then I will receive bet response of error "SystemError"
	
Scenario: 02.e - Player cannot place bet with invalid session token
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add invalid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports"
	Then I will receive bet response of error "InvalidToken"
	
Scenario: 02.f - Player cannot place bet with invalid product wallet
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports" with invalid product wallet
	Then I will receive bet response of error "InvalidArguments"
	
Scenario: 02.g - Player cannot place bet with invalid prefix
	Given player "TP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	When I place bet of $-10 for game "imesports" with invalid prefix
	Then I will receive bet response of error "InvalidArguments"
	
Scenario: 03.a - Win bet
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	When I use action SettleFT to bet for $15
	Then I will receive bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And the round will be Closed
		
Scenario: 03.b - Lose bet
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	When I use action SettleFT to bet for $0
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And the round will be Closed
		
Scenario: 03.c - Can win bet without player login
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given player logout
	When I use action SettleFT to bet for $15
	Then I will receive bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And the round will be Closed
		
Scenario: 03.d - Resend win bet
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given I use action SettleFT to bet for $15
		And balance now being $105
	When I resend the settle bet
	Then I will receive bet response of no error and balance of $105
	
Scenario: 04.a - Cancel bet
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	When I use action PlaceBetRefund to bet for $10
	Then I will receive bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And the round will be Open
		
Scenario: 04.b - Can cancel bet without player login
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given player logout
	When I use action PlaceBetRefund to bet for $10
	Then I will receive bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And the round will be Open

Scenario: 04.c - Cancel settle
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given I use action SettleFT to bet for $15
		And balance now being $105
	When I use action UnsettleFT to bet for $-15
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "CancelTransaction" for $-15
		And the round will be Open
		
Scenario: 04.d - Cancel cancel
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given I use action CancelSingleWager to bet for $10
		And balance now being $100
	When I use action UncancelSingleWager to bet for $-10
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "CancelTransaction" for $-10
		And the round will be Open
		
Scenario: 05.a - Resettle
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given I use action SettleFT to bet for $10
		And balance now being $100
	Given I use action UnsettleFT to bet for $-10
		And balance now being $90
	When I use action SettleFT to bet for $0
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And the round will be Closed
				
Scenario: 05.b - Settle after cancel cancel
	Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
		And add valid session token in redis for game "imesports"
	Given I place bet of $-10 for game "imesports"
		And balance now being $90
	Given I use action CancelSingleWager to bet for $10
		And balance now being $100
	Given I use action UncancelSingleWager to bet for $-10
		And balance now being $90
	When I use action SettleFT to bet for $15
	Then I will receive bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And the round will be Closed

Scenario: 06.a - Settle 2 place bets
	Given players place bets:
		| Username | Brand | Currency | Platform | Balance | GameId    | Amount |
		| TP01cny  | GPI   | CNY      | Mobile   | 100     | imesports | 10     |
		| RP01cny  | GPI   | CNY      | Desktop  | 100     | imesports | 5      |
	When I use action to bets
		| Username | Currency | ActionType  | GameId    | Amount |
		| TP01cny  | CNY      | SettleFT    | imesports | 0      |
		| RP01cny  | CNY      | SettleFT    | imesports | 15     |
	Then I will receive successful response
		And there will be 2 transactions in the response of error "Success"
		And all game action related database records will be created
		And the balance of each user will be matched
		| Username | Balance |
		| TP01cny  | 90      |
		| RP01cny  | 110     |

Scenario: 06.b - Cancel 2 place bets
	Given players place bets:
		| Username | Brand | Currency | Platform | Balance | GameId    | Amount |
		| TP01cny  | GPI   | CNY      | Mobile   | 100     | imesports | 10     |
		| RP01cny  | GPI   | CNY      | Desktop  | 100     | imesports | 5      |
	When I use action to bets
		| Username | Currency | ActionType     | GameId    | Amount |
		| TP01cny  | CNY      | PlaceBetRefund | imesports | -10    |
		| RP01cny  | CNY      | PlaceBetRefund | imesports | -5     |
	Then I will receive successful response
		And there will be 2 transactions in the response of error "Success"
		And all game action related database records will be created
		And the balance of each user will be matched
		| Username | Balance |
		| TP01cny  | 100     |
		| RP01cny  | 100     |

Scenario: 06.c - Settle and  cancel place bets
	Given players place bets:
		| Username | Brand | Currency | Platform | Balance | GameId    | Amount |
		| TP01cny  | GPI   | CNY      | Mobile   | 100     | imesports | 10     |
		| RP01cny  | GPI   | CNY      | Desktop  | 100     | imesports | 5      |
	When I use action to bets
		| Username | Currency | ActionType     | GameId    | Amount |
		| TP01cny  | CNY      | SettleFT       | imesports | 20     |
		| RP01cny  | CNY      | PlaceBetRefund | imesports | -5     |
	Then I will receive successful response
		And there will be 2 transactions in the response of error "Success"
		And all game action related database records will be created
		And the balance of each user will be matched
		| Username | Balance |
		| TP01cny  | 110     |
		| RP01cny  | 100     |