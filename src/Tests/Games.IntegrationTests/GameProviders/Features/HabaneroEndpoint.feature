@GameIntegration
@Habanero

Feature: Habanero Endpoint
	Test Habanero Endpoint

Scenario: 01.a - Get authorized player details
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get authorize response and player balance will be $125

Scenario Outline: 01.b - Get authorized player details with invalid authorization
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
    When I call authorize endpoint with username="<username>" and passkey="<passkey>" and brandid="<brandid>"
    Then I will get error authorize response and no player balance

    Examples: 
    | username       | passkey                                | brandid                              |
    | habanero       | slPrV84jbxM8jIH9nUYBAndgVyjF5nU6-wrong | a5df0ada-7b6a-eb11-9889-00155db5435a |
    | habanero-wrong | slPrV84jbxM8jIH9nUYBAndgVyjF5nU6       | a5df0ada-7b6a-eb11-9889-00155db5435a |
    | habanero       | slPrV84jbxM8jIH9nUYBAndgVyjF5nU6       | a5df0ada-7b6a-eb11-9889-wrong0000000 |

Scenario: 01.c - Get player details after player logout
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get error authorize response and no player balance

@ChangeGameProviderStatus
Scenario: 01.d - Cannot get player details if the game provider status is set to maintenance
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And game provider has status set to "Maintenance"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get error authorize response and no player balance

@ChangeBrandStatus
Scenario: 01.e - Cannot get player details if the brand status is set to maintenance
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And brand has status set to "Maintenance"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get error authorize response and no player balance

@ChangeGameProviderStatus
Scenario: 01.f - Can get Test player details if the game provider status is set to maintenance
   Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And game provider has status set to "Maintenance"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get authorize response and player balance will be $125

@ChangeBrandStatus
Scenario: 01.j - Can get Test player details if the brand status is set to maintenance
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And brand has status set to "Maintenance"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get authorize response and player balance will be $125

@ChangeGameProviderStatus
Scenario: 01.k - Cannot get Test player details if the game provider status is set to archived
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And game provider has status set to "Archived"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get error authorize response and no player balance

@ChangeBrandStatus
Scenario: 01.l - Cannot get Test player details if the brand status is set to archived
    Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And brand has status set to "Archived"
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get error authorize response and no player balance

Scenario: 02.a - Place desktop bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 02.b - Place mobile bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Mobile"
        And player having balance of $100
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 02.c - Player cannot place bet without login
	Given player "TP01_cny" of brand "GPI" without login
        And player having balance of $100
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response with authorization error and no balance

Scenario: 02.e - Place bet with insufficient balance
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $5
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response no funds error and balance of $5

Scenario: 02.f - Place bet with duplicate transaction ID
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
    When I resend the same bet
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "PlaceBet" for $-10

Scenario: 02.g - Place bet using expired player token
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $125
        And player logout
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response with authorization error and no balance

Scenario: 03.a - Win bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
    When I settle bet for $15
    Then I will receive bet response of no error and balance of $105
        And player wallet database record will be for $105
        And game action will be recorded in the database of type "WinBet" for $15

Scenario: 03.b - Win bet after player logout
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And player logout
    When I settle bet for $15
    Then I will receive bet response of no error and balance of $105

Scenario: 03.c - Lose bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
    When I settle bet for $0
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LoseBet" for $0

Scenario: 03.d - Free bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I placed bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    When I got a free bet for $10
    Then I will receive bet response of no error and balance of $115
        And player wallet database record will be for $115
        And game action will be recorded in the database of type "FreeBet" for $10

Scenario: 03.e - Settle bet with duplicate transaction ID
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And settle bet for $15
    When I resend the same settle bet
    Then I will receive bet response of no error and balance of $105
        And player wallet database record will be for $105
        And game action will be recorded in the database of type "WinBet" for $15

Scenario: 04.a - Cancel transaction
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
    When I cancel transaction
    Then I will receive "Cancelled" cancel response and balance of $100
        And player wallet database record will be for $100
        And game action will be recorded in the database of type "CancelTransaction" for $10

Scenario: 04.b - Cancel transaction after player logout
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And player logout
    When I cancel transaction
    Then I will receive "Cancelled" cancel response and balance of $100
        And player wallet database record will be for $100
        And game action will be recorded in the database of type "CancelTransaction" for $10

Scenario: 04.c - Cancel non-existent transaction
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I cancel non-existent transaction for game "AcesandEights1Hand"
    Then I will receive "TransactionNotFound" cancel response and balance of $100
	
Scenario: 05 - Cannot Settle a Cancelled bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And I cancel transaction
        And balance now being $100
    When I settle bet for $15
    Then I will receive bet response debit/credit error and no balance

@DisablePlayer
Scenario: 06.a - Can not place bet when player is disabled
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And disable the player
    When I place bet of $10 for game "AcesandEights1Hand"
    Then I will receive bet response with authorization error and no balance

@DisablePlayer
Scenario: 06.b - Can settle bet when player is disabled
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And disable the player
    When I settle bet for $15
    Then I will receive bet response of no error and balance of $105

Scenario: 07.a - Retry win bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
    When I retry settle bet for $15
    Then I will receive bet response of no error and balance of $105
        And player wallet database record will be for $105
        And game action will be recorded in the database of type "WinBet" for $15

Scenario: 07.b - Retry win bet after player logout
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
        And player logout
    When I retry settle bet for $15
    Then I will receive bet response of no error and balance of $105

Scenario: 07.c - Retry lose bet
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And balance now being $90
    When I retry settle bet for $0
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And game action will be recorded in the database of type "LoseBet" for $0

Scenario: 08.a - Place and win bet in one request
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    Then I will receive bet response of no error and balance of $105
        And player wallet database record will be for $105
        And #1 game action will be recorded in the database of type "PlaceBet" for $-10
        And #2 game action will be recorded in the database of type "WinBet" for $15

Scenario: 08.b - Place and lose bet in one request
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet for $10 and settle it for $0 for game "AcesandEights1Hand"
    Then I will receive bet response of no error and balance of $90
        And player wallet database record will be for $90
        And #1 game action will be recorded in the database of type "PlaceBet" for $-10
        And #2 game action will be recorded in the database of type "LoseBet" for $0

Scenario: 08.c - Cannot place and settle bet after player logout
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And player logout
    When I place bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    Then I will receive bet response with authorization error and no balance

Scenario: 08.d - Place and settle bet with insufficient balance
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $5
    When I place bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    Then I will receive bet response no funds error and balance of $5

Scenario: 08.e - Place and settle bet with duplicate transaction IDs
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I placed bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    When I resend place and settle bet
    Then I will receive bet response of no error and balance of $105

Scenario: 08.f - Place and settle bet using expired player token
    Given player having balance of $100
        And player logout
    When I place bet for $10 and settle it for $15 for game "AcesandEights1Hand"
    Then I will receive bet response with authorization error and no balance

Scenario: 08.g - Place bet and settle with invalid reference Id
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
    When I place bet for $10 and settle invalid reference Id for game "AcesandEights1Hand"
    Then I will receive bet response debit/credit error and balance of $100
        And #1 game action will NOT be recorded in the database
        And #2 game action will NOT be recorded in the database

Scenario: 09.a - Query existent game action status
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
    When I query bet existence
    Then I get "true" query result

Scenario: 09.b - Query non-existent game action status
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And I set non-existent game action
    When I query bet existence
    Then I get "false" query result

Scenario: 09.c - Query existent game action status after player logout
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
        And player logout
    When I query bet existence
    Then I get "true" query result

Scenario: 09.d - Query existent game action status with empty token
    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
        And player having balance of $100
        And I place bet of $10 for game "AcesandEights1Hand"
    When I query bet existence without token
    Then I get "true" query result