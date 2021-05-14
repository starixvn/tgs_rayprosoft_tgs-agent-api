DROP FUNCTION IF EXISTS dbo.report_get_round_aggregates;
CREATE OR REPLACE FUNCTION dbo.report_get_round_aggregates(
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
	select 
	p.brand_id "BrandId",
	p.agent_id "AgentId", 
	g.game_provider_id "GameProviderId",
	p.player_code "PlayerCode", 
	g.type "GameType", 
	p.username "Username", 
	_hour "Hour", 
	_minute "Minute", 
	sum(r.bet_amount) "BetAmount", 
	sum(r.valid_bet_amount) "ValidBetAmount",
	sum(r.turnover) "Turnover", 
	sum(r.win_loss) "WinLoss", 
	_report_date "ReportDate"
    from bet.rounds r 
	join dbo.games g on r.game_id = g.id
	join dbo.players p on r.player_code = p.player_code
    where r.closed_on >= _from and r.closed_on < _to and r.reopened_on is null
	group by p.brand_id, g.game_provider_id, p.player_code, p.agent_id, p.username,g.type;
	
END;
$$