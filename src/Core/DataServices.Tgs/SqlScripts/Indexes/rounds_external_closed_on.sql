DROP INDEX IF EXISTS bet.ix_rounds_external_closed_on;

CREATE INDEX ix_rounds_external_closed_on ON bet.rounds USING brin (external_closed_on);