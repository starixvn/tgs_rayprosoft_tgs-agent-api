DROP FUNCTION IF EXISTS dbo.report_get_player_no_second_deposit_details;
CREATE OR REPLACE FUNCTION dbo.report_get_player_no_second_deposit_details(
    _brand_id UUID,
    _agent_id UUID,
	_from timestamptz, 
	_to timestamptz)
RETURNS TABLE (
	 "Username" character varying,
	 "FirstDepositOn" timestamptz,
	 "FirstDepositAmount" decimal, 
	 "TotalDepositAmount" decimal,
	 "TotalWithdrawAmount" decimal,
	 "DepositSubtractWithdraw" decimal,
	 "IpAddress" character varying) 
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 
WITH depositinterval
     AS (SELECT df.player_code,
                CASE
                  WHEN df.actual_amount IS NULL THEN df.amount
                  ELSE df.actual_amount
                END                          AMOUNT,
                df.created_on,
                Row_number()
                  OVER (
                    partition BY df.player_code
                    ORDER BY df.created_on ASC) ROW_NUM
         FROM   dbo.deposit_forms df
		 JOIN   dbo.players p ON p.player_code = df.player_code
         WHERE  df.brand_id = _brand_id
                AND p.agent_id = _agent_id
                AND df.status = 8000
                AND df.created_on >= _from
                AND df.created_on < _to),
     checkfirstdeposit
     AS (SELECT T.player_code,
                T.amount,
                T.created_on,
                COUNT(d) total_COUNT
         FROM   depositinterval T
                LEFT JOIN dbo.deposit_forms D
                       ON D.player_code = T.player_code
                          AND D.created_on < _from
                          AND D.status = 8000
         WHERE  row_num = 1
         GROUP  BY 1,
                   2,
				   3),
     totaldeposit
     AS (SELECT T2.player_code,
                T2.amount,
                T2.created_on,
                Sum(D2.amount) TOTAL_DEPOSIT
         FROM   checkfirstdeposit T2
                LEFT JOIN dbo.deposit_forms D2
                       ON D2.player_code = T2.player_code
                          AND D2.status = 8000
         WHERE  total_COUNT = 1
         GROUP  BY 1,
                   2,
		           3),
     totalwithdraw
     AS (SELECT T3.*,
                COALESCE(Sum(CASE
                               WHEN W.actual_amount IS NULL THEN W.amount
                               ELSE W.actual_amount
                             END), 0) TOTAL_WITHDRAW
         FROM   totaldeposit T3
                LEFT JOIN dbo.withdraw_forms W
                       ON W.user_id = T3.player_code
                          AND W.status = 9000
         GROUP  BY 1,
                   2,
                   3,
		           4)
SELECT username       AS "Username",
	   created_on     AS "FirstDepositOn",
       amount         AS "FirstDepositAmount",
       total_deposit  AS "TotalDepositAmount",
       total_withdraw AS "TotalWithdrawAmount",
	   total_deposit - total_withdraw AS "DepositSubtractWithdraw",
       ip             AS "IpAddress"
FROM   (SELECT P.username,
               T4.*,
               I.ip,
               Rank()
                 OVER (
                   partition BY I.player_code
                   ORDER BY record_on ASC) IP_RANK
        FROM   totalwithdraw T4
               JOIN dbo.players P
                 ON P.player_code = T4.player_code
               LEFT JOIN dbo.player_ip_records I
                      ON I.player_code = T4.player_code
                         AND ip_type = 1) T5
WHERE  ip_rank = 1 ;
END;
$$