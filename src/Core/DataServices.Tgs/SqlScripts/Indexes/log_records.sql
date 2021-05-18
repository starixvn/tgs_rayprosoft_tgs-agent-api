  CREATE INDEX ix_log_records_recorded_on
  ON log.log_records (recorded_on DESC);

  CLUSTER log.log_records USING ix_log_records_recorded_on;