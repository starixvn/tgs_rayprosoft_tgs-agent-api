-- unique index of players for lower case username, instead of alternative key
CREATE UNIQUE INDEX ux_players_brand_id_email
	ON dbo.players (brand_id,lower(email));