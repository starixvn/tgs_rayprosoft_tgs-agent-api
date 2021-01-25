  CREATE INDEX ix_licensees_code_name_description_operational_email
  ON dbo.licensees (code,name,description,operational_email);

  CREATE INDEX ix_trgm_licensees_code_name_description_operational_email
  ON dbo.licensees USING gin (code gin_trgm_ops,name gin_trgm_ops,description gin_trgm_ops,description gin_trgm_ops,operational_email gin_trgm_ops);