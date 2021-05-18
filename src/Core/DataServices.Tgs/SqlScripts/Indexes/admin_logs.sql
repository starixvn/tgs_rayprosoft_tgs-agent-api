  CREATE INDEX ix_admin_logs_recorded_on
  ON log.admin_logs (recorded_on DESC);

  CLUSTER log.admin_logs USING ix_admin_logs_recorded_on;