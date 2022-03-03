DROP FUNCTION IF EXISTS DBO.report_generate_game_provider_aggregate_by_external_closed_on;
CREATE OR REPLACE FUNCTION DBO.report_generate_game_provider_aggregate_by_external_closed_on(
    _from timestamptz, 
	_to timestamptz,
    _game_provider_id uuid,
	_audit_log_id integer,
	_created_on timestamptz)
RETURNS void
LANGUAGE plpgsql
AS $$

	BEGIN
	WITH rounds AS(
		SELECT
		r.game_provider_id,
		r.currency_code,
		date_trunc('hour', r.external_closed_on) report_date,
		COUNT(1) transaction_count,
		COALESCE(SUM(bet_amount), 0) bet_amount,
		COALESCE(SUM(valid_bet_amount), 0) valid_bet_amount,
		COALESCE(SUM(turnover), 0) turnover,
		COALESCE(SUM(win_loss), 0) win_loss,
		COALESCE(SUM(settle_amount), 0) settle_amount,
		COALESCE(SUM(commission), 0) commission
		FROM bet.rounds r
		WHERE r.external_closed_on >= _from and r.external_closed_on < _to and r.game_provider_id = _game_provider_id
		GROUP BY 1,2,3
		),
		insert_rows AS(
		INSERT INTO report.game_provider_aggregates (audit_log_id,game_provider_id,currency_code,transaction_count,bet_amount,valid_bet_amount,turnover,win_loss,settle_amount,commission,report_date,created_on)
		SELECT _audit_log_id,game_provider_id,currency_code,transaction_count,bet_amount,valid_bet_amount,turnover,win_loss,settle_amount,commission,report_date,_created_on
		FROM rounds
		)

   		INSERT INTO report.game_provider_aggregate_records (audit_log_id,game_provider_id,"data",created_on)
		SELECT _audit_log_id,_game_provider_id,(select case when count(1) > 0 then jsonb_agg(rounds) else '[]' end FROM rounds),_created_on;
		
	END;
	$$