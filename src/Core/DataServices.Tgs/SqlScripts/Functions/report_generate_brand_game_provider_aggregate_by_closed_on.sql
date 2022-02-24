DROP FUNCTION IF EXISTS DBO.report_generate_brand_game_provider_aggregate_by_closed_on;
CREATE OR REPLACE FUNCTION DBO.report_generate_brand_game_provider_aggregate_by_closed_on(
    _from timestamptz, 
	_to timestamptz,
    _brand_id uuid,
	_audit_log_id integer,
	_created_on timestamptz)
RETURNS void
LANGUAGE plpgsql
AS $$

	BEGIN
	WITH rounds AS(
		SELECT 
		r1.brand_id,
		r1.game_provider_id,
		bc.currency_code,
		r1.transaction_count,
		r1.bet_amount,
		r1.valid_bet_amount,
		r1.turnover,
		r1.win_loss,
		r1.settle_amount,
		r1.commission,
		r1.report_date FROM
		(
		SELECT
		r.brand_id,
		r.game_provider_id,
		date_trunc('hour', r.closed_on) report_date,
		COUNT(1) transaction_count,
		COALESCE(SUM(r.bet_amount), 0) bet_amount,
		COALESCE(SUM(r.valid_bet_amount), 0) valid_bet_amount,
		COALESCE(SUM(r.turnover), 0) turnover,
		COALESCE(SUM(r.win_loss), 0) win_loss,
		COALESCE(SUM(r.settle_amount), 0) settle_amount,
		COALESCE(SUM(r.commission), 0) commission
		FROM bet.rounds r
		WHERE r.closed_on >= _from and r.closed_on < _to and r.brand_id = _brand_id
		GROUP BY 1,2,3) r1
		left join LATERAL (select brand_id,currency_code from dbo.brand_currencies where brand_id=r1.brand_id limit 1) bc on r1.brand_id = bc.brand_id
		),
		insert_rows AS(
		INSERT INTO report.brand_game_provider_aggregates (audit_log_id,brand_id,game_provider_id,currency_code,transaction_count,bet_amount,valid_bet_amount,turnover,win_loss,settle_amount,commission,report_date,created_on)
		SELECT _audit_log_id,brand_id,game_provider_id,currency_code,transaction_count,bet_amount,valid_bet_amount,turnover,win_loss,settle_amount,commission,report_date,_created_on
		FROM rounds
		)

		INSERT INTO report.brand_game_provider_aggregate_records (audit_log_id,"data",created_on)
		SELECT _audit_log_id,(select case when count(1) > 0 then jsonb_agg(rounds) else '[]' end FROM rounds),_created_on;
	END;
	$$