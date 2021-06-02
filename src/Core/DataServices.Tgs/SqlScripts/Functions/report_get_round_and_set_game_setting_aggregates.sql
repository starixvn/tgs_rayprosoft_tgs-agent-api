DROP FUNCTION IF EXISTS DBO.report_get_round_and_set_game_setting_aggregates;
CREATE OR REPLACE FUNCTION DBO.report_get_round_and_set_game_setting_aggregates(
    _from timestamptz, 
	_to timestamptz,
	_report_date timestamptz,
	_hour integer,
	_minute integer)
	RETURNS TABLE(
     "BrandId" uuid, 
	 "AgentId" uuid, 
	 "GameProviderId" uuid,
	 "PlayerCode" varchar,
	 "GameId" integer,
	 "GameType" smallint, 
	 "Username" varchar, 
	 "Hour" integer, 
	 "Minute" integer, 
	 "BetAmount" decimal, 
	 "ValidBetAmount" decimal, 
	 "Turnover" decimal, 
	 "WinLoss" decimal, 
	 "ReportDate" timestamptz)
LANGUAGE plpgsql
AS $$
	BEGIN
		RETURN QUERY
		WITH rounds AS(
		select
		r.id "round_id",
		p.brand_id,
		p.agent_id,
		g.game_provider_id,
		p.player_code,
		g.id "game_id",
		g.type "game_type",
		p.username,
		r.bet_amount,
		r.valid_bet_amount,
		r.turnover,
		r.win_loss,
		CASE WHEN bg.effective_betting_percentage is null THEN 100 ELSE bg.effective_betting_percentage END AS effective_betting_percentage
		from bet.rounds r
		join dbo.games g on r.game_id = g.id
		join dbo.players p on r.player_code = p.player_code
		left join dbo.brand_games bg on bg.brand_id = r.brand_id and bg.game_id = r.game_id
		where r.closed_on >= _from and r.closed_on < _to and r.reopened_on is null
		),
		rows AS (select
		brand_id,
		agent_id,
		game_provider_id,
		player_code,
		game_id,
		game_type,
		username,
		_hour "hour",
		_minute "minute",
		sum(bet_amount) "bet_amount",
		sum(valid_bet_amount) "valid_bet_amount",
		sum(turnover * effective_betting_percentage / 100)  "turnover",
		sum(win_loss) "win_loss",
		_report_date "report_date"
		from rounds
		group by brand_id, game_provider_id, player_code, agent_id, username,game_id,game_type),
		update_rounds as (
		  UPDATE bet.rounds r SET effective_betting_percentage = br.effective_betting_percentage
		  FROM rounds br where r.id in (br.round_id)
		)

		select
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
		turnover "Turnover",
		win_loss "WinLoss",
		report_date "ReportDate"
		from rows;

	END;
	$$