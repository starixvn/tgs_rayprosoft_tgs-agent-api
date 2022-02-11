DROP FUNCTION IF EXISTS DBO.report_generate_brand_game_provider_aggregate_by_closed_on;
CREATE OR REPLACE FUNCTION DBO.report_generate_brand_game_provider_aggregate_by_closed_on(
    _from timestamptz, 
	_to timestamptz,
    _brand_id uuid)
RETURNS numeric
LANGUAGE plpgsql
AS $$
	BEGIN
	RETURN QUERY
		WITH rounds AS(
		SELECT
		r.brand_id,
		r.game_provider_id,
		date_trunc('hour', r.closed_on at time zone 'Asia/Taipei') report_date,
		COUNT(1),
		SUM(r.bet_amount) bet_amount,
		SUM(r.valid_bet_amount) valid_bet_amount,
		SUM(r.turnover) turnover,
		SUM(r.win_loss) win_loss
		FROM bet.rounds r
		WHERE r.closed_on >= _from and r.closed_on < _to and r.brand_id = _brand_id
		GROUP BY 1,2,3
		),
		insert_rows AS(
		INSERT INTO report.round_aggregates (brand_id,agent_id,player_code,game_provider_id,game_type,hour,minute,bet_amount,valid_bet_amount,original_turnover,turnover,win_loss,report_date,created_on,game_id)
		SELECT brand_id,agent_id,player_code,game_provider_id,game_type,hour,minute,bet_amount,valid_bet_amount,original_turnover,turnover,win_loss,report_date,created_on,game_id
		FROM rows
		)

		SELECT
		brand_id "BrandId",
		agent_id "AgentId",
		game_provider_id "GameProviderId",
		player_code "PlayerCode",
		game_id "GameId",
		game_type "GameType",
		username "Username",
		hour "Hour",
		minute "Minute",
		bet_amount "BetAmount",
		valid_bet_amount "ValidBetAmount",
		original_turnover "OriginalTurnover",
		turnover "Turnover",
		win_loss "WinLoss",
		report_date "ReportDate"
		from rows;
	END;
	$$