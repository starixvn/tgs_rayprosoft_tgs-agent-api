  CREATE INDEX ix_payments_name
  ON dbo.payments (name);

  CREATE INDEX ix_trgm_payments_name
  ON dbo.payments USING gin (name gin_trgm_ops);
