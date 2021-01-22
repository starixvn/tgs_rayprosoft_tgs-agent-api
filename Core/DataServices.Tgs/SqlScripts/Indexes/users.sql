  CREATE INDEX ix_users_username
  ON dbo.users (username);

  CREATE INDEX ix_trgm_users_username
  ON dbo.users USING gin (username gin_trgm_ops);
