CREATE OR REPLACE FUNCTION bet.game_action_referenced_game_action_exists(
	game_provider_id uuid, 
	player_code varchar,
	external_transaction_id varchar,
	external_round_id varchar)
RETURNS TABLE(
	"Exists" bool)
LANGUAGE plpgsql
AS $$
BEGIN
	
	RETURN QUERY
	select 
		CASE WHEN count(1) > 0 THEN true ELSE false END as "Exists"
	from bet.game_actions ga 
	join bet.rounds r on r.id = ga.round_id 
	where r.external_round_id = $4
	and r.game_provider_id = $1
	and r.player_code = $2
	and ga.external_transaction_id = $3;
	
END;
$$