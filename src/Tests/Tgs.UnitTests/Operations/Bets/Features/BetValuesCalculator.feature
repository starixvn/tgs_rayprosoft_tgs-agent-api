@Unit
@BetValuesCalculator
Feature: Bet Values Calculator

	Given a sequence of game action 
	calculates BetAmount, SettleAmount, Turnover, WinLoss, Unsettled Bets, TieBetAdjustment


Scenario: 01 - Bet, then Lose, cancel the Loss, then Win.
	Given initial balance of $500
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type              | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet          | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | LoseBet           | 0      | 400          | 100      | 100     | 0                | -100           | Yes              |          |
	| 3  | 2      | CancelTransaction | 0      | 400          | -100     | -100    | 0                | 100            | No               |          |
	| 4  | 1      | WinBet            | 200    | 600          | 100      | -100    | 0                | -100           | Yes              |          |
	|    |        | Totals            | 100    | 600          | 100      | -100    | 0                | 0              |                  |          |         
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 200

Scenario: 02 - Lose the bet, Cancel the Loss, then Cancel Bet. (Void Round)
	Given initial balance of $500
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type              | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet          | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | LoseBet           | 0      | 400          | 100      | 100     | 0                | -100           | Yes              |          |
	| 3  | 2      | CancelTransaction | 0      | 400          | -100     | -100    | 0                | 100            | No               |          |
	| 4  | 1      | CancelTransaction | 100    | 500          | 0        | 0       | 0                | -100           | Yes              |          |
	|    |        | Totals            | 0      | 500          | 0        | 0       | 0                | 0              |                  |          |
	Then the final bet amount is 100 and valid bet amount is 0 and settle amount is 0

Scenario: 03 - Bet, then Tie for Baccarat (does not count towards turnover).
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | TieBet   | 100    | 500          | 0        | 0       | 100              | -100           | Yes              |          |
	|    |        | Totals   | 0      | 500          | 0        | 0       | 100              | 0              |                  |          |
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 100

Scenario: 04 - Bet, then Tie for 3 Card Poker (counts towards turnover).
	Given initial balance of $500
		And game with turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | TieBet   | 100    | 500          | 100      | 0       | 0                | -100           | Yes              |          |
	|    |        | Totals   | 0      | 500          | 100      | 0       | 0                | 0              |                  |          |
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 100

Scenario: 05 - Bet, then Free Bet, then Win.
	Given initial balance of $500
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | FreeBet  | 100    | 500          | 0        | 0       | 0                | 0              |                  |          |
	| 3  | 1      | WinBet   | 200    | 700          | 100      | -200    | 0                | -100           | Yes              |          |
	|    |        | Totals   | 200    | 700          | 100      | -200    | 0                | 0              |                  |          |
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 300

Scenario: 06 - Bet, then Win, then Free Bet. 
	Given initial balance of $500
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | WinBet   | 200    | 600          | 0        | 0       | 0                | 0              |                  |          |
	| 3  | 1      | FreeBet  | 100    | 700          | 100      | -200    | 0                | -100           | Yes              |          |
	|    |        | Totals   | 200    | 700          | 100      | -200    | 0                | 0              |                  |          |
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 300

Scenario: 07 - Multiple Bets with multiple settlements, and NO tie turnover, then cancellation. 
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type              | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet          | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  |        | PlaceBet          | -50    | 350          | 0        | 0       | 0                | 50             |                  |          |
	| 3  |        | PlaceBet          | -20    | 330          | 0        | 0       | 0                | 20             |                  |          |
	| 4  | 1      | TieBet            | 100    | 430          | 0        | 0       | 0                | 0              |                  |          |
	| 5  | 2      | WinBet            | 75     | 505          | 0        | 0       | 0                | 0              |                  |          |
	| 6  | 3      | TieBet            | 20     | 525          | 50       | -25     | 120              | -170           | Yes              |          |
	| 7  | 5      | CancelTransaction | -75    | 450          | -50      | 25      | 0                | 170            | No               |          |
	| 8  | 2      | LoseBet           | 0      | 450          | 50       | 50      | 120              | -170           | Yes              |          |
	Then the final bet amount is 170 and valid bet amount is 170 and settle amount is 120

Scenario: 08 - Multiple Bets with multiple settlements, and tie turnover, then cancellation. 
	Given initial balance of $500
		And game with turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type              | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet          | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  |        | PlaceBet          | -50    | 350          | 0        | 0       | 0                | 50             |                  |          |
	| 3  |        | PlaceBet          | -20    | 330          | 0        | 0       | 0                | 20             |                  |          |
	| 4  | 1      | TieBet            | 100    | 430          | 0        | 0       | 0                | 0              |                  |          |
	| 5  | 2      | WinBet            | 75     | 505          | 0        | 0       | 0                | 0              |                  |          |
	| 6  | 3      | TieBet            | 20     | 525          | 170      | -25     | 0                | -170           | Yes              |          |
	| 7  | 5      | CancelTransaction | -75    | 450          | -170     | 25      | 0                | 170            | No               |          |
	| 8  | 2      | LoseBet           | 0      | 450          | 170      | 50      | 0                | -170           | Yes              |          |
	Then the final bet amount is 170 and valid bet amount is 170 and settle amount is 120

Scenario: 09 - Multiple Bets with multiple settlements, and tie turnover. 
	Given initial balance of $500
		And game with turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  |        | PlaceBet | -50    | 350          | 0        | 0       | 0                | 50             |                  |          |
	| 3  |        | PlaceBet | -20    | 330          | 0        | 0       | 0                | 20             |                  |          |
	| 4  | 1      | TieBet   | 100    | 430          | 0        | 0       | 0                | 0              |                  |          |
	| 5  | 2      | WinBet   | 75     | 505          | 0        | 0       | 0                | 0              |                  |          |
	| 6  | 3      | TieBet   | 20     | 525          | 170      | -25     | 0                | -170           | Yes              |          |
	Then the final bet amount is 170 and valid bet amount is 170 and settle amount is 195

Scenario: 10 - Multiple Bets with multiple settlements, and NO tie turnover. 		   
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type     | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  |        | PlaceBet | -50    | 350          | 0        | 0       | 0                | 50             |                  |          |
	| 3  |        | PlaceBet | -20    | 330          | 0        | 0       | 0                | 20             |                  |          |
	| 4  | 1      | TieBet   | 100    | 430          | 0        | 0       | 0                | 0              |                  |          |
	| 5  | 2      | WinBet   | 75     | 505          | 0        | 0       | 0                | 0              |                  |          |
	| 6  | 3      | TieBet   | 20     | 525          | 50       | -25     | 120              | -170           | Yes              |          |
	Then the final bet amount is 170 and valid bet amount is 170 and settle amount is 195

Scenario: 11 - Bet, then Tie for Baccarat (does not count towards turnover), then cancel Tie.
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type              | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | PlaceBet          | -100   | 400          | 0        | 0       | 0                | 100            |                  |          |
	| 2  | 1      | TieBet            | 100    | 500          | 0        | 0       | 100              | -100           | Yes              |          |
	| 3  | 2      | CancelTransaction | -100   | 400          | 0        | 0       | 0                | 100            | No               |          |
	| 4  | 1      | WinBet            | 75     | 475          | 100      | 25      | 0                | -100           | Yes              |          |
	|    |        | Totals            | -25    | 475          | 100      | 25      | 100              | -0             |                  |          |
	Then the final bet amount is 100 and valid bet amount is 100 and settle amount is 75

Scenario: 12 - Free Bet in a single round
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type    | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | FreeBet | 100    | 600          | 0        | -100    | 0                | 0              | Yes              |          |
	|    |        | Totals  | 100    | 600          | 0        | -100    | 0                | 0              |                  |          |
	Then the final bet amount is 0 and valid bet amount is 0 and settle amount is 100

Scenario: 13 - Win Jackpot
	Given initial balance of $500
		And game with no turnover on tie bet
	When I perform the following actions I will see the expected results
	| Id | Ref Id | Type       | Amount | Post Balance | Turnover | WinLoss | TieBetAdjustment | Unsettled Bets | Is Closing Round | Bet Type |
	| 1  |        | WinJackpot | 100    | 600          | 0        | -100    | 0                | 0              | Yes              |          |
	|    |        | Totals     | 100    | 600          | 0        | -100    | 0                | 0              |                  |          |
	Then the final bet amount is 0 and valid bet amount is 0 and settle amount is 100