  CREATE INDEX ix_payment_channels_name
  ON dbo.payment_channels (name);

  CREATE INDEX ix_trgm_payment_channels_name
  ON dbo.payment_channels USING gin (name gin_trgm_ops);
