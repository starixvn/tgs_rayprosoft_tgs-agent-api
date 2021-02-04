  CREATE INDEX ix_resellers_code_name_description_operational_email
  ON dbo.resellers (code,name,description,operational_email);

  CREATE INDEX ix_trgm_resellers_code_name_description_operational_email
  ON dbo.resellers USING gin (code gin_trgm_ops,name gin_trgm_ops,description gin_trgm_ops,description gin_trgm_ops,operational_email gin_trgm_ops);