CREATE OR REPLACE FUNCTION bet.game_action_get_game_action_external_ids(
	game_provider_id uuid, 
	player_code varchar,
	external_round_id varchar,
	game_action_types int[])
RETURNS TABLE(
	"ExternalBetId" varchar,
	"ExternalTransactionId" varchar,
	"ExternalRoundId" varchar, 
	"ExternalGameActionGroupId" varchar, 
	"GameActionType" smallint, 
	"Amount" decimal,
	"RecordedOn" timestamptz, 
	"ExternalReferencedTransactionId" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
	
	RETURN QUERY
	select 
		ga.external_bet_id as "ExternalBetId",
		ga.external_transaction_id as "ExternalTransactionId",
		r.external_round_id as "ExternalRoundId",
		ga.external_game_action_group_id as "ExternalGameActionGroupId",
		ga."type" as "GameActionType",
		ga.amount as "Amount",
		ga.recorded_on as "RecordedOn",
		(select g.external_transaction_id from bet.game_actions g where g.id = ga.referenced_game_action_id) as "ExternalReferencedTransactionId"
	from bet.game_actions ga 
	join bet.rounds r on r.id = ga.round_id 
	where r.external_round_id = $3
	and r.game_provider_id = $1
	and r.player_code = $2
	and ga.game_provider_configuration_id = r.game_provider_configuration_id 
	and ga.wallet_id  = r.wallet_id 
	and ga."type" = any($4);
	
END;
$$