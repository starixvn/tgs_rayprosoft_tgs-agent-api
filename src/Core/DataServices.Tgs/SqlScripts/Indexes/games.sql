  CREATE INDEX ix_games_name
  ON dbo.games (name);

  CREATE INDEX ix_trgm_games_name
  ON dbo.games USING gin (name gin_trgm_ops);
