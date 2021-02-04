  CREATE INDEX ix_payment_methods_name
  ON dbo.payment_methods (name);

  CREATE INDEX ix_trgm_payment_methods_name
  ON dbo.payment_methods USING gin (name gin_trgm_ops);
