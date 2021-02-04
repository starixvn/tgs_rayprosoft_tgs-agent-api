  CREATE INDEX ix_admin_logs_recorded_on
  ON dbo.admin_logs (recorded_on DESC);

  CLUSTER dbo.admin_logs USING ix_admin_logs_recorded_on;