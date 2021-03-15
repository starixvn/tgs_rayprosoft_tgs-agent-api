@GameIntegration
@Habanero

Feature: Habanero Endpoint
	Test Habanero Endpoint

Scenario: 01.a - Get authorized player details
	Given player "TP01_cny" of brand "GPI" with "CNY" login via "Desktop"
		And player having balance of $125
	When I call authorize endpoint with username="habanero" and passkey="slPrV84jbxM8jIH9nUYBAndgVyjF5nU6" and brandid="a5df0ada-7b6a-eb11-9889-00155db5435a"
    Then I will get authorize response and player balance will be $125
