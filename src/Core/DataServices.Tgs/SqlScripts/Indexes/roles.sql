  CREATE INDEX ix_roles_name
  ON dbo.roles (name);

  CREATE INDEX ix_trgm_roles_name
  ON dbo.roles USING gin (name gin_trgm_ops);
