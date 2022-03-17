DROP FUNCTION IF EXISTS DBO.report_get_round_and_set_game_setting_aggregates;
CREATE OR REPLACE FUNCTION DBO.report_get_round_and_set_game_setting_aggregates(
    _from timestamptz, 
	_to timestamptz)
	RETURNS TABLE(
     "RoundId" uuid, 
	 "PlayerCode" varchar,
	 "Turnover" decimal, 
	 "EffectiveBettingPercentage" decimal)
LANGUAGE plpgsql
AS $$
	BEGIN
		RETURN QUERY
		WITH rounds AS(
		select
		r.id "round_id",
		r.player_code,
		r.turnover,
		CASE WHEN bg.effective_betting_percentage is null THEN 100 ELSE bg.effective_betting_percentage END AS effective_betting_percentage
		from bet.rounds r
		left join dbo.brand_games bg on bg.brand_id = r.brand_id and bg.game_id = r.game_id
		where r.closed_on >= _from and r.closed_on < _to and r.reopened_on is null
		),
		update_rounds as (
		  UPDATE bet.rounds r SET effective_betting_percentage = br.effective_betting_percentage
		  FROM rounds br where r.id in (br.round_id)
		)

		select
		round_id "RoundId",
		player_code "PlayerCode",
		turnover "Turnover",
		win_loss "WinLoss",
		effective_betting_percentage "EffectiveBettingPercentage"
		from rounds;

	END;
	$$