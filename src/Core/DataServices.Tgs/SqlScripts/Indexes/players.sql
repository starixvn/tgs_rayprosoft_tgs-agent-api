-- unique index of players for lower case username, instead of alternative key
CREATE UNIQUE INDEX ux_players_brand_id_username 
	ON dbo.players (brand_id, (lower(username)));

CREATE INDEX ix_players_username
	ON dbo.players (username);

CREATE INDEX ix_trgm_players_username
	ON dbo.players USING gin (username gin_trgm_ops);  