  CREATE INDEX ix_brands_code_name_description_operational_email
  ON dbo.brands (code,name,description,operational_email);

  CREATE INDEX ix_trgm_brands_code_name_description_operational_email
  ON dbo.brands USING gin (code gin_trgm_ops,name gin_trgm_ops,description gin_trgm_ops,description gin_trgm_ops,operational_email gin_trgm_ops);