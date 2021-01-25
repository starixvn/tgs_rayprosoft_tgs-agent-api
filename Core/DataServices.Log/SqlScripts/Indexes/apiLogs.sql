  CREATE INDEX ix_api_logs_recorded_on
  ON dbo.api_logs (recorded_on DESC);

  CLUSTER dbo.api_logs USING ix_api_logs_recorded_on;