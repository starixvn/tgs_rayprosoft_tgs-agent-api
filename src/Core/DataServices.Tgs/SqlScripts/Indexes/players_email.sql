-- unique index of players for lower case email, instead of alternative key
CREATE UNIQUE INDEX ux_players_email_email 
	ON dbo.players (lower(email));