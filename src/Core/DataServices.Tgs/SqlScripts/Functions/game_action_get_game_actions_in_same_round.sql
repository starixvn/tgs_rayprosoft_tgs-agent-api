CREATE OR REPLACE FUNCTION bet.game_action_get_game_actions_in_same_round(
	game_provider_id uuid, 
	player_code varchar,
	external_round_id varchar,
	game_id int)
RETURNS TABLE(
	"GameActionId" uuid,
	"ReferenceGameActionId" uuid,
	"ExternalTransactionId" varchar, 
	"ExternalGameActionGroupId" varchar,
	"Turnover" decimal, 
	"WinLoss" decimal, 
	"UnsettledBets" decimal,
	"IsClosingRound" bool, 
	"ClosingRoundExplicitRequest" bool, 
	"Type" smallint, 
	"RoundId" uuid, 
	"GameId" int,
	"RoundStatus" smallint,
	"ExternalRoundId" varchar,
	"Amount" decimal,
	"GameActionRecordedOn" timestamptz,
	"GameActionSentOn" timestamptz,
	"BetType" varchar,
	"WalletId" uuid)
LANGUAGE plpgsql
AS $$
BEGIN
	
	set lock_timeout to 3000;
	
	RETURN QUERY
	with round as
	(
		select 
			r.id,
			r.game_id,
			r.status,
			r.external_round_id,
			r.wallet_id,
			r.created_on 
		from bet.rounds r
		where r.external_round_id = $3
		and r.player_code = $2
		and r.game_provider_id = $1
		and r.game_id = $4
		for update
	)
	select 
		ga.id as "GameActionId",
		ga.referenced_game_action_id as "ReferenceGameActionId",
		ga.external_transaction_id as "ExternalTransactionId",
		ga.external_game_action_group_id as "ExternalGameActionGroupId",
		ga.turnover as "Turnover",
		ga.win_loss as "WinLoss",
		ga.unsettled_bets as "UnsettledBets",
		ga.is_closing_round as "IsClosingRound",
		ga.closing_round_explicit_request as "ClosingRoundExplicitRequest",
		ga."type" as "Type",
		r.id as "RoundId",
		r.game_id as "GameId",
		r.status as "RoundStatus",
		r.external_round_id as "ExternalRoundId",
		ga.amount as "Amount",
		ga.recorded_on as "GameActionRecordedOn",
		ga.sent_on as "GameActionSentOn",
		ga.bet_type as "BetType",
		r.wallet_id as "WalletId"
	from round r 
	join bet.game_actions ga on ga.round_id = r.id 
	where ga.recorded_on >= (r.created_on - 5 * interval '1 minute')
	order by ga.recorded_on asc;
	
END;
$$