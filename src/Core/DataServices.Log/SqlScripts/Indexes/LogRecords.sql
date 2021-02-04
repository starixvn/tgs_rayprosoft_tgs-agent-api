  CREATE INDEX ix_log_records_recorded_on
  ON dbo.log_records (recorded_on DESC);

  CLUSTER dbo.log_records USING ix_log_records_recorded_on;