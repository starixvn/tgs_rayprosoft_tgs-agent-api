DROP INDEX IF EXISTS bet.ix_game_actions_external_transaction_id;

CREATE INDEX ix_game_actions_external_transaction_id ON bet.game_actions USING btree (external_transaction_id);