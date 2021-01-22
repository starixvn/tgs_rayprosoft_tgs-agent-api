  CREATE INDEX ix_game_providers_name
  ON dbo.game_providers (name);

  CREATE INDEX ix_trgm_game_providers_name
  ON dbo.game_providers USING gin (name gin_trgm_ops);
