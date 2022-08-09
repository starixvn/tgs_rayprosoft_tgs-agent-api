CREATE OR REPLACE FUNCTION dbo.report_get_player_account_change_records(
	_from timestamptz, 
	_to timestamptz,
	_player_code varchar,
	_transaction_type integer default  null, 
	_game_action_type integer default  null,
	_page_size integer default 10,
	_page_number integer default 1)
RETURNS TABLE(
     "TransactionId" uuid, 
	 "GameActionId" uuid, 
	 "Amount" decimal,
	 "Balance" decimal,
	 "TransactionType" smallint, 
	 "GameActionType" smallint, 
	 "RecordedOn" timestamptz,
	 "TotalCount" bigint, 
	 "Description" text,
	 "ExternalTransactionId" text)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 
	SELECT 
	TRANSACTION_ID as "TransactionId",
	GAME_ACTION_ID as "GameActionId",
	AMOUNT as "Amount",
	BALANCE as "Balance",
	TRANSACTION_TYPE as "TransactionType",
	GAME_ACTION_TYPE as "GameActionType",
	RECORDED_ON as "RecordedOn",
	count(*) over() as "TotalCount",
	DESCRIPTION as "Description",	
	EXTERNAL_TRANSACTION_ID as "ExternalTransactionId"
FROM
				(SELECT ID AS TRANSACTION_ID,
						NULL AS GAME_ACTION_ID,
						AMOUNT,
						BALANCE,
						TYPE AS TRANSACTION_TYPE,
						NULL AS GAME_ACTION_TYPE,
						RECORDED_ON,
						DESCRIPTION,
						EXTERNAL_TRANSACTION_ID
					FROM DBO.TRANSACTIONS
					WHERE RECORDED_ON >= _from
									AND RECORDED_ON < _to
									AND (
											(case 
											 when _transaction_type = 0 then ("type" in (0, 1100))
											 when _transaction_type = 1 then ("type" in (1, 2100))
											 when _transaction_type = 9 then ("type" in (9,2300))
											 when _transaction_type IS null then "type" != 2200
											 else
											 "type" = _transaction_type
									         end) AND _game_action_type IS null)
									AND PLAYER_CODE = _player_code
					UNION ALL SELECT NULL AS TRANSACTION_ID,
						ID AS GAME_ACTION_ID,
						AMOUNT,
						BALANCE,
						NULL AS TRANSACTION_TYPE,
						TYPE AS GAME_ACTION_TYPE,
						RECORDED_ON,
						NULL as DESCRIPTION,
						NULL as EXTERNAL_TRANSACTION_ID
					FROM BET.GAME_ACTIONS
					WHERE RECORDED_ON >= _from
									AND RECORDED_ON < _to
									AND ((_game_action_type IS NULL
														OR "type" = _game_action_type) AND _transaction_type IS NULL)
									AND PLAYER_CODE = _player_code) AS A
ORDER BY A.RECORDED_ON DESC
OFFSET (_page_number-1) * _page_size
LIMIT _page_size;
	
END;
$$