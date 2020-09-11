
### SQL Query used to view GGR, Turnover, Unsettled Bets for player

```sql

DECLARE @Username varchar(128) = 'as044401'
SELECT 
	l.Code as Licensee,
	b.Code as Brand,
	w.CurrencyCode,
	gp.Code as GameProvider,
	@Username as Player,
	COUNT(DISTINCT r.Id) as Rounds,
	-SUM(CASE WHEN t.Id IS NOT NULL AND ga.Type = 0 /*PlaceBet*/ THEN t.Amount ELSE 0 END) as Risk, -- the sum of all BET action transaction amounts
	SUM(ga.Turnover) as Turnover,
	SUM(ga.GrossGamingRevenue) as GrossGamingRevenue,
	SUM(ga.UnsettledBets) as UnsettledBets,
	CAST(
        ROUND(
            (
                SUM(ga.GrossGamingRevenue) -- GrossGamingRevenue
                / -- divided by
                -SUM(ga.Turnover) -- Turnover
            )*100
        , 2) 
    as FLOAT) as GrossGamingRevenuePercent
FROM GameActions ga
INNER JOIN Rounds r ON ga.RoundId = r.Id
INNER JOIN Games g ON r.GameId = g.Id
INNER JOIN GameProviders gp ON g.GameProviderId = gp.Id
INNER JOIN Wallets w ON ga.WalletId = w.Id
INNER JOIN Players p ON w.PlayerId = p.Id
INNER JOIN Brands b ON p.BrandId = b.Id
INNER JOIN Licensees l ON b.LicenseeId = l.Id
LEFT OUTER JOIN Transactions t ON ga.TransactionId = t.Id
WHERE p.Username = @Username
GROUP BY l.Code, b.Code, w.CurrencyCode, gp.Code
```
