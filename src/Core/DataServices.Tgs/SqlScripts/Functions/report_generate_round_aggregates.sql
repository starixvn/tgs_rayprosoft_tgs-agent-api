DROP FUNCTION IF EXISTS DBO.report_generate_round_aggregates;
CREATE OR REPLACE FUNCTION DBO.report_generate_round_aggregates(
    _from timestamptz, 
	_to timestamptz,
    _report_date timestamptz,
	_created_on timestamptz,
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
		SELECT
		r.id,
		r.bet_amount,
		r.valid_bet_amount,
		r.turnover,
		r.win_loss,
		r.game_id,
		r.player_code,
		r.brand_id,
		r.effective_betting_percentage AS round_effective_betting_percentage,
		bg.effective_betting_percentage AS brand_game_effective_betting_percentage
		FROM bet.rounds r
		left join dbo.brand_games bg on bg.brand_id = r.brand_id and bg.game_id = r.game_id
		WHERE r.closed_on >= _from and r.closed_on < _to and r.reopened_on is null
		)
		,rows AS (select
		p.brand_id,
		p.agent_id,
		g.game_provider_id,
		p.player_code,
		g.id "game_id",
		g.type "game_type",
		p.username,
		_hour "hour",
		_minute "minute",
		sum(r.bet_amount) "bet_amount",
		sum(r.valid_bet_amount) "valid_bet_amount",
		sum(CASE 
		WHEN r.round_effective_betting_percentage is not null THEN r.turnover * r.round_effective_betting_percentage / 100
		WHEN r.brand_game_effective_betting_percentage is not null THEN r.turnover * r.brand_game_effective_betting_percentage / 100 
		ELSE r.turnover END) "turnover",
		sum(r.win_loss) "win_loss",
		_report_date "report_date",
		_created_on "created_on"
		from rounds r
		join dbo.games g on r.game_id = g.id
		join dbo.players p on r.player_code = p.player_code
		group by p.brand_id, g.game_provider_id, p.player_code, p.agent_id, p.username,g.id,g.type),
		insert_rows AS(
		INSERT INTO report.round_aggregates (brand_id,agent_id,player_code,game_provider_id,game_type,hour,minute,bet_amount,valid_bet_amount,turnover,win_loss,report_date,created_on,game_id)
		SELECT brand_id,agent_id,player_code,game_provider_id,game_type,hour,minute,bet_amount,valid_bet_amount,turnover,win_loss,report_date,created_on,game_id
		FROM rows
		),
		update_rounds AS(
		UPDATE bet.rounds r SET effective_betting_percentage = CASE WHEN rounds.brand_game_effective_betting_percentage is not null THEN rounds.brand_game_effective_betting_percentage ELSE 100 END 
		FROM rounds
		WHERE r.id = rounds.id AND rounds.round_effective_betting_percentage is null
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
		turnover "Turnover",
		win_loss "WinLoss",
		report_date "ReportDate"
		from rows;
	END;
	$$