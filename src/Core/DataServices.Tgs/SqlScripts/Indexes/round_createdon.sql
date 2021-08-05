DROP INDEX IF EXISTS bet.ix_rounds_created_on;

CREATE INDEX ix_rounds_created_on ON bet.rounds USING brin (created_on);