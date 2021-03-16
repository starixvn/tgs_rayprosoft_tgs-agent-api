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

Scenario: 01.c - Get player details without login
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


#Scenario: 02.a - Place desktop bet
#    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Desktop"
#        And player having balance of $100
#    When I place bet of $10 for game "AcesandEights1Hand"
#    Then I will receive bet response of no error and balance of $90
#        And player wallet database record will be for $90
#        And game action will be recorded in the database of type "PlaceBet" and $-10 transaction amount
#        And game platform will be recorded in the database of type "Desktop"
#
#Scenario: 02.b - Place mobile bet
#    Given player "RP01_cny" of brand "GPI" with "CNY" login via "Mobile"
#        And player having balance of $100
#    When I place bet of $10 for game "AcesandEights1Hand"
#    Then I will receive bet response of no error and balance of $90
#        And player wallet database record will be for $90
#        And game action will be recorded in the database of type "PlaceBet" and $-10 transaction amount
#        And game platform will be recorded in the database of type "Mobile"

#Scenario: 02.c - Cannot place bet with deauthorized player token
#    Given authorized player token
#        And player having balance of $100
#        And deauthorized player tokens
#    When I place bet of $10 for game "AcesandEights1Hand"
#    Then I will receive bet response authorization error and no balance
#
#Scenario: 02.e - Place bet with insufficient balance
#    Given authorized player token
#        And player having balance of $5
#    When I place bet of $10 for game "AcesandEights1Hand"
#    Then I will receive bet response no funds error and balance of $5
#
#Scenario: 02.f - Place bet with duplicate transaction ID
#    Given authorized player token
#        And player having balance of $100
#        And placed bet of $10 for game "AcesandEights1Hand"
#    When I place the same bet again
#    Then I will receive bet response debit/credit error and no balance
#
#Scenario: 02.g - Place bet using expired player token
#    Given player having balance of $100
#        And player expired token
#    When I place bet of $10 for game "AcesandEights1Hand"
#    Then I will receive bet response authorization error and no balance
