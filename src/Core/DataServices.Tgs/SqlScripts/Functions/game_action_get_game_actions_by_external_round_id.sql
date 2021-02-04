CREATE OR REPLACE FUNCTION bet.game_action_get_game_actions_by_external_round_id(
	game_provider_id uuid, 
	player_code varchar,
	external_round_id varchar,
	game_id int)
RETURNS TABLE(
	"Id" uuid,       
	"ExternalBetId" varchar,
	"ExternalTransactionId" varchar,
	"RoundId" uuid,
	"ExternalRoundId" varchar,
	"ExternalGameActionGroupId" varchar,
	"GameActionType" smallint,
	"Amount" decimal,
	"RecordedOn" timestamptz,
	"ReferencedGameActionId" uuid,
	"RoundStatus" smallint,
	"RoundBetAmount" decimal,
	"RoundWinLoss" decimal,
	"Commission" decimal,
	"TieBetAdjustment" decimal,
	"RoundClosedOn" timestamptz)
LANGUAGE plpgsql
AS $$
BEGIN
	
	RETURN QUERY
	select 
		ga.id as "Id",       
		ga.external_bet_id as "ExternalBetId",
		ga.external_transaction_id as "ExternalTransactionId",
		r.id as "RoundId",
		r.external_round_id as "ExternalRoundId",
		ga.external_game_action_group_id as "ExternalGameActionGroupId",
		ga."type" as "GameActionType",
		ga.amount as "Amount",
		ga.recorded_on as "RecordedOn",
		ga.referenced_game_action_id as "ReferencedGameActionId",
		r.status as "RoundStatus",
		r.bet_amount as "RoundBetAmount",
		r.win_loss as "RoundWinLoss",
		r.commission as "Commission",
		r.tie_bet_adjustment as "TieBetAdjustment",
		r.closed_on as "RoundClosedOn"
	from bet.game_actions ga 
	join bet.rounds r on r.id = ga.round_id 
	where r.external_round_id = $3
	and r.game_provider_id = $1
	and r.player_code = $2
	and r.game_id = $4;
	
END;
$$