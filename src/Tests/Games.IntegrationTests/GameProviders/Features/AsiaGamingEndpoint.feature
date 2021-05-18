@GameIntegration
@AsiaGaming

Feature: AsiaGaming Endpoint
	Test AsiaGaming Endpoint

Scenario: 01.a - Place desktop bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_BAC"
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10
        
Scenario: 01.b - Place mobile bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Mobile"
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_BAC"
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10
        
Scenario: 01.c - Player cannot place bet without login
	Given player "RP01cny" of brand "GPI" without login
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_BAC"
    Then I will receive bet response of INVALID_SESSION and balance of $0
    
Scenario: 01.d - Place bet with insufficient balance
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $5
    When I place bet of $10 for game "AGIN_DSP_BAC"
    Then I will receive bet response of INSUFFICIENT_FUNDS and balance of $5
    
Scenario: 01.e - Place bet with duplicate transaction ID
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
    When I resend the same bet
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 01.f - Place bet using expired player token
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
    When I place bet of $10 for game "AGIN_DSP_BAC"
    Then I will receive bet response of INVALID_SESSION and balance of $0
    
Scenario: 02.a - Lock desktop bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_ROU"
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LockAmount" for $-10
        
Scenario: 02.b - Lock mobile bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Mobile"
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_ROU"
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LockAmount" for $-10
        
Scenario: 02.c - Player cannot lock bet without login
	Given player "RP01cny" of brand "GPI" without login
        And player having balance of $100
    When I place bet of $10 for game "AGIN_DSP_ROU"
    Then I will receive bet response of INVALID_SESSION and balance of $0
    
Scenario: 02.d - Lock bet with insufficient balance
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $5
    When I place bet of $10 for game "AGIN_DSP_ROU"
    Then I will receive bet response of INSUFFICIENT_FUNDS and balance of $5
    
Scenario: 02.e - Lock bet with duplicate transaction ID
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_ROU"
    When I resend the same bet
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LockAmount" for $-10

Scenario: 02.f - Lock bet using expired player token
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
    When I place bet of $10 for game "AGIN_DSP_ROU"
    Then I will receive bet response of INVALID_SESSION and balance of $0
    
Scenario: 03.a - Bet response for ROU game
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_ROU"
        And balance now being $90
    When I send bet response
		| PlayType | Amount | Val |
		| 101      | 4      | 1   |
		| 103      | 6      |     |
    Then I will receive bet response of OK and balance of $0
        And player wallet database record will be for $90
        And round will have record of type "LockAmount" in the database for $-10
        And round will have 2 record of type "PlaceBet" in the database
        
Scenario: 03.b - Bet response for BAC game
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
    When I send bet response with no record
    Then I will receive bet response of OK and balance of $0
        
Scenario: 04.a - Win bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
    When I settle bet for win amount $15
    Then I will receive bet response of OK and balance of $115
        And player wallet database record will be for $115
        And game action will be recorded in the database of type "WinBet" for $25
        
Scenario: 04.b - Win bet after player logout
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
        And player logout
    When I settle bet for win amount $15
    Then I will receive bet response of OK and balance of $115

Scenario: 04.c - Win bet with different platform token
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
        And player logout
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Mobile"
    When I settle bet for win amount $15
    Then I will receive bet response of OK and balance of $115
    
Scenario: 04.d - Lose bet
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
    When I settle bet for win amount $-10
    Then I will receive bet response of OK and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LoseBet" for $0

Scenario: 04.e - Settle bet with duplicate transaction ID
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
        And settle bet for win amount $15
    When I resend the same settle bet
    Then I will receive bet response of OK and balance of $115
        And player wallet database record will be for $115
        And game action will be recorded in the database of type "WinBet" for $25
        
Scenario: 04.f - Settle bet for ROU game
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_ROU"
        And balance now being $90
        And I send bet response
		| PlayType | Amount | Val |
		| 101      | 4      | 1   |
		| 103      | 6      |     |
    When I settle bet
		| PlayType | WinAmount | RoundFinish |
		| 101      | 8         | false       |
		| 103      | -6        | true        |
    Then I will receive bet response of OK and balance of $102
        And player wallet database record will be for $102
        And game action id contains 101 will be recorded in the database of type "WinBet" for $12
        And game action id contains 103 will be recorded in the database of type "LoseBet" for $0
        
Scenario: 05.a - Cancel transaction
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
    When I cancel transaction
    Then I will receive bet response of OK and balance of $100
        And player wallet database record will be for $100
        And game action will be recorded in the database of type "CancelTransaction" for $10
        And the round will be closed
        
Scenario: 05.b - Cancel transaction after player logout
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AGIN_DSP_BAC"
        And balance now being $90
        And player logout
    When I cancel transaction
    Then I will receive bet response of OK and balance of $100
        And player wallet database record will be for $100
        And game action will be recorded in the database of type "CancelTransaction" for $10
        And the round will be closed

Scenario: 05.c - Cancel non-existent transaction
    Given player "RP01cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I cancel non-existent transaction for game "AGIN_DSP_BAC"
    Then I will receive bet response of INVALID_TRANSACTION and balance of $100
	