CREATE INDEX IF NOT EXISTS ix_rounds_external_round_id_composite 
ON bet.rounds USING btree (external_round_id, player_code, game_provider_id, game_id);
