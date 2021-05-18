  CREATE INDEX ix_api_logs_recorded_on
  ON log.api_logs (recorded_on DESC);

  CLUSTER log.api_logs USING ix_api_logs_recorded_on;