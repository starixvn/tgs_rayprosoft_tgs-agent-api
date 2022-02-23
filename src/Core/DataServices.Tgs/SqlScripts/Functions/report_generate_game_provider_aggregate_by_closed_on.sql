DROP FUNCTION IF EXISTS DBO.report_generate_game_provider_aggregate_by_closed_on;
CREATE OR REPLACE FUNCTION DBO.report_generate_game_provider_aggregate_by_closed_on(
    _from timestamptz, 
	_to timestamptz,
    _brand_id uuid,
	_created_on timestamptz)
RETURNS void
LANGUAGE plpgsql
AS $$
	BEGIN
	INSERT INTO report.game_provider_aggregates (brand_id,game_provider_id,currency_code,transaction_count,bet_amount,valid_bet_amount,turnover,win_loss,external_closed_on,created_on)
	SELECT r2.brand_id,r2.game_provider_id,r2.currency_code,r2.transaction_count,r2.bet_amount,r2.valid_bet_amount,r2.turnover,r2.win_loss,r2.report_date,_created_on
	FROM (SELECT r1.brand_id,r1.game_provider_id,bc.currency_code,r1.transaction_count,r1.bet_amount,r1.valid_bet_amount,r1.turnover,r1.win_loss,r1.report_date FROM
		(SELECT
		r.brand_id,
		r.game_provider_id,
		date_trunc('hour', r.external_closed_on) report_date,
		COUNT(1) transaction_count,
		SUM(r.bet_amount) bet_amount,
		SUM(r.valid_bet_amount) valid_bet_amount,
		SUM(r.turnover) turnover,
		SUM(r.win_loss) win_loss
		FROM bet.rounds r
		WHERE r.external_closed_on >= _from and r.external_closed_on < _to and r.brand_id = _brand_id
		GROUP BY 1,2,3) r1
		left join LATERAL (select brand_id,currency_code from dbo.brand_currencies where brand_id=r1.brand_id limit 1) bc on r1.brand_id = bc.brand_id
		) r2;
	END;
	$$