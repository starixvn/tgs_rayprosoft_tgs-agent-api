@GameIntegration
@SBOBET

Feature: SBOBET Endpoint
	Test SBOBET Endpoint

Scenario: 01.a - Get player balance
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 01.b - Get player balance without login
	Given player "TP01_cny" of brand "GPI" without login
		And player having balance of $125
	When I request the player balance
	Then player balance response will be for $125

Scenario: 02.a - Place desktop bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

Scenario: 02.b - Place mobile bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Mobile"
		And player having balance of $100
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Mobile"

Scenario: 02.c - Player cannot place bet without login
	Given player "TP01_cny" of brand "GPI" without login
		And player having balance of $100
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of error "PlayerNotLogin"

@ChangeGameProviderStatus
Scenario: 02.d - Real player cannot place bet with game provider with maintenance status
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given game provider has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of error "UnderMaintenance"

@ChangeGameProviderStatus
Scenario: 02.e - GPI Test player can place bet with game provider with maintenance status
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given game provider has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

@ChangeGameProviderConfigurationStatus
Scenario: 02.f - Real player cannot place bet with game provider configuration with maintenance status
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given game provider configuration has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of error "UnderMaintenance"

@ChangeGameProviderConfigurationStatus
Scenario: 02.g - TGS Test player can place bet with game provider configuration with maintenance status
	Given player "TP01_cny" of brand "GPI" with "TGS" login via "Desktop"	
		And player having balance of $100
	Given game provider configuration has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

@ChangeGameStatus
Scenario: 02.h - Real player cannot place bet with game with maintenance status
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given game "1_1" has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of error "UnderMaintenance"

@ChangeGameStatus
Scenario: 02.i - TGS Test player place bet with game with maintenance status
	Given player "TP01_cny" of brand "GPI" with "TGS" login via "Desktop"	
		And player having balance of $100
	Given game "1_1" has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

@ChangeBrandStatus
Scenario: 02.j - Real player cannot place bet with brand with maintenance status
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given brand has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of error "UnderMaintenance"

@ChangeBrandStatus
Scenario: 02.k - GPI Test player place bet with brand with maintenance status
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"	
		And player having balance of $100
	Given brand has status set to "Maintenance"
	When I place bet of $10 for game "1_1"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

Scenario: 03.a - Win bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 03.b - Can win bet without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given player logout
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderStatus
Scenario: 03.c - Can win bet with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider has status set to "Maintenance"
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderConfigurationStatus
Scenario: 03.d - Can win bet with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider configuration has status set to "Maintenance"
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameStatus
Scenario: 03.e - Can win bet with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game "1_1" has status set to "Maintenance"
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeBrandStatus
Scenario: 03.f - Can win bet with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given brand has status set to "Maintenance"
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 03.g - Resend win bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $15
		And balance now being $105
	When I resend the win bet
	Then I will receive settle bet response of error "BetAlreadySettled"

Scenario: 03.h - Win bet with unknown game
	Given player "RP01_cny" of brand "TGS" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I win bet for game "1_0" for $15 
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 04.a - Lose bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 04.b - Can lose bet without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given player logout
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderStatus
Scenario: 04.c - Can lose bet with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider has status set to "Maintenance"
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderConfigurationStatus
Scenario: 04.d - Can lose bet with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider configuration has status set to "Maintenance"
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameStatus
Scenario: 04.e - Can lose bet with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game "1_1" has status set to "Maintenance"
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeBrandStatus
Scenario: 04.f - Can lose bet with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given brand has status set to "Maintenance"
	When I lose bet
	Then I will receive settle bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "LoseBet" for $0
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 04.g - Resend lose bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I lose bet
		And balance now being $90
	When I resend the lose bet
	Then I will receive settle bet response of error "BetAlreadySettled"

Scenario: 05.a - Tie bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 05.b - Can tie bet without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given player logout
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderStatus
Scenario: 05.c - Can tie bet with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider has status set to "Maintenance"
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderConfigurationStatus
Scenario: 05.d - Can tie bet with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider configuration has status set to "Maintenance"
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameStatus
Scenario: 05.e - Can tie bet with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game "1_1" has status set to "Maintenance"
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeBrandStatus
Scenario: 05.f - Can tie bet with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given brand has status set to "Maintenance"
	When I tie bet for $10
	Then I will receive settle bet response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "TieBet" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 05.g - Resend tie bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I tie bet for $10
		And balance now being $100
	When I resend the tie bet
	Then I will receive settle bet response of error "BetAlreadySettled"

Scenario: 06.a - Cancel bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 06.b - Can cancel bet without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given player logout
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderStatus
Scenario: 06.c - Can cancel bet with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider has status set to "Maintenance"
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameProviderConfigurationStatus
Scenario: 06.d - Can cancel bet with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game provider configuration has status set to "Maintenance"
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeGameStatus
Scenario: 06.e - Can cancel bet with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given game "1_1" has status set to "Maintenance"
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

@ChangeBrandStatus
Scenario: 06.f - Can cancel bet with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given brand has status set to "Maintenance"
	When I cancel transaction
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 06.g - Resend cancel bet
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I cancel transaction
		And balance now being $100
	When I resend cancel transaction
	Then I will receive cancel bet response of error "BetAlreadyCanceled"

Scenario: 06.h - Cancel bet with unknown game
	Given player "RP01_cny" of brand "TGS" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	When I cancel transaction for game "1_0"
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And game action will be recorded in the database of type "CancelTransaction" for $10
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario: 07.a - Void round
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

Scenario: 07.b - Can void round without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given player logout
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

@ChangeGameProviderStatus
Scenario: 07.c - Can void round with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given game provider has status set to "Maintenance"
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

@ChangeGameProviderConfigurationStatus
Scenario: 07.d - Can void round with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given game provider configuration has status set to "Maintenance"
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

@ChangeGameStatus
Scenario: 07.e - Can void round with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given game "1_1" has status set to "Maintenance"
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

@ChangeBrandStatus
Scenario: 07.f - Can void round with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given brand has status set to "Maintenance"
	When I cancel all transactions
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

Scenario: 07.g - Resend cancel bet of void round
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given I cancel all transactions
		And balance now being $100
	When I resend cancel transaction
	Then I will receive cancel bet response of error "BetAlreadyCanceled"

Scenario: 07.h - Void round with unknown game
	Given player "RP01_cny" of brand "TGS" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	When I cancel all transactions for game "1_0"
	Then I will receive cancel response of no error and balance of $100
		And player wallet database record will be for $100
		And the round has no valid bets
		And the round will be closed

Scenario: 08.a - Cancel settlement
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

Scenario: 08.b - Can cancel settle without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given player logout
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameProviderStatus
Scenario: 08.c - Can cancel settle with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given game provider has status set to "Maintenance"
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameProviderConfigurationStatus
Scenario: 08.d - Can cancel settle with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given game provider configuration has status set to "Maintenance"
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameStatus
Scenario: 08.e - Can cancel settle with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
	Given game "1_1" has status set to "Maintenance"
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

@ChangeBrandStatus
Scenario: 08.f - Can cancel settle with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given brand has status set to "Maintenance"
	When I cancel the settle transaction
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the last settle game action recorded in the database will be canceled
		And the round will be opened

Scenario: 08.g - Resend cancel settle
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $10
		And balance now being $100
	Given I cancel the settle transaction
		And balance now being $90
	When I resend cancel settlement transaction
	Then I will receive rollback response of error "BetAlreadyRollback"

# Note for 09 case:
# When a void round is rollback, the place bet is accepted again and the orignal settlement is discarded
Scenario: 09.a - Rollback void round
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	When I cancel the cancellation
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

Scenario: 09.b - Rollback void round without player login
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given player logout
	When I cancel the cancellation	
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameProviderStatus
Scenario: 09.c - Can rollback void round with game provider status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given game provider has status set to "Maintenance"
	When I cancel the cancellation	
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameProviderConfigurationStatus
Scenario: 09.d - Can rollback void round with game provider configuration status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given game provider configuration has status set to "Maintenance"
	When I cancel the cancellation	
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

@ChangeGameStatus
Scenario: 09.e - Can rollback void round with game status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given game "1_1" has status set to "Maintenance"
	When I cancel the cancellation	
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

@ChangeBrandStatus
Scenario: 09.f - Can rollback void round with brand status set to maintenance
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given brand has status set to "Maintenance"
	When I cancel the cancellation	
	Then I will receive rollback response of no error and balance of $90
		And player wallet database record will be for $90
		And the cancellation of game action recorded in the database will be canceled
		And the round will be opened

Scenario: 09.g - Cannot resend rollback void round 
	Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And balance now being $90
	Given I win bet for $20
		And balance now being $110
	Given I cancel all transactions
		And balance now being $100
	Given I cancel the cancellation	
		And balance now being $90
	When I resend cancel the cancellation transaction
	Then I will receive rollback response of error "InvalidGameAction"
	
Scenario: 10.a - Get bet status of place bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	When I get bet status
	Then I will receive bet response of no error
		| Field   | Value   |
		| status  | running |
		| stake   | 10      |
		| winloss | 0       |
	
Scenario: 10.b - Get bet status of place bet with unknown game
	Given player "TP01_cny" of brand "TGS" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	When I get bet status for game "1_0"
	Then I will receive bet response of no error
		| Field   | Value   |
		| status  | running |
		| stake   | 10      |
		| winloss | 0       |

Scenario: 10.c - Get bet status of win bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	Given I win bet for $20
		And player having balance of $110
	When I get bet status
	Then I will receive bet response of no error
		| Field   | Value   |
		| status  | settled |
		| stake   | 10      |
		| winloss | 20      |

Scenario: 10.d - Get bet status of win bet with unknown game
	Given player "TP01_cny" of brand "TGS" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	Given I win bet for $20
		And player having balance of $110
	When I get bet status for game "1_0"
	Then I will receive bet response of no error
		| Field   | Value   |
		| status  | settled |
		| stake   | 10      |
		| winloss | 20      |

Scenario: 10.e - Get bet status of lose bet
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	Given I lose bet
		And player having balance of $90
	When I get bet status
	Then I will receive bet response of no error
		| Field   | Value   |
		| status  | settled |
		| stake   | 10      |
		| winloss | 0       |

Scenario: 10.f - Get bet status of cancel transaction
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "1_1"
		And player having balance of $90
	Given I cancel transaction
		And player having balance of $100
	When I get bet status
	Then I will receive bet response of no error
		| Field   | Value |
		| status  | void  |
		| stake   | 0     |
		| winloss | 0     |

Scenario: 11.a - Place desktop bet of sub game
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	When I place bet of $10 for game "5_201600"
	Then I will receive bet response of no error and balance of $90
		And player wallet database record will be for $90
		And game action will be recorded in the database of type "PlaceBet" for $-10
		And game platform will be recorded in the database of type "Desktop"

Scenario: 11.b - Win bet of sub game
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given I place bet of $10 for game "5_201600"
		And balance now being $90
	When I win bet for $15
	Then I will receive settle bet response of no error and balance of $105
		And player wallet database record will be for $105
		And game action will be recorded in the database of type "WinBet" for $15
		And game platform will be recorded in the database of type "Desktop"
		And the round will be closed

Scenario:  12 - Place bet with the same external round id in differnet game
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $100
	Given set random external round id
	Given I place bet with set round id of $10 for game "1_1"
		And I win bet for $20
		And player having balance of $110
	When I place bet with set round id of $10 for game "5_201608"
		And I lose bet
	Then I will receive settle bet response of no error and balance of $100 
		And the round with set round id belongs to games
			| GameId   |
			| 1_1      |
			| 5_201608 |

Scenario: 13 - Cancel place bets with batch
	Given players place bets:
		| Username | Brand | Currency | Platform | Balance | GameId   | BetAmount |
		| TP01_cny | GPI   | CNY      | Mobile   | 100     | 1_1      | 10        |
		| RP01_cny | GPI   | CNY      | Desktop  | 100     | 5_201608 | 5         |
	When I submit the batch of bets to be canceled
	Then I will receive successful response
		And there will be 2 transactions in the response
		And there will be 0 errors in the response
		And all cancel bet related database records will be created and the balance of each user will be matched as original