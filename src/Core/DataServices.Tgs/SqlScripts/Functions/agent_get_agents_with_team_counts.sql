DROP FUNCTION IF EXISTS dbo.agent_get_agents_with_team_counts;
CREATE OR REPLACE FUNCTION dbo.agent_get_agents_with_team_counts(
    _brand_id UUID,
	_username text,
	_page_size integer,
	_page_number integer, 
	_sort_by text, 
	_is_desc bool,
    _status smallint default null,
	_type smallint default null,
	_level_type smallint default null)
RETURNS TABLE (
	 "Id" UUID, 
	 "BrandId" UUID, 
	 "Username" character varying, 
	 "Status" smallint, 
	 "AgentStatus" integer,
	 "Type" smallint,
	 "LevelType" integer,
	 "Balance" decimal,	 
	 "CommissionPercentage" decimal,
	 "DividendPercentage" decimal,
	 "LiveDealerRebatePercentage" decimal, 
	 "SlotRebatePercentage" decimal,
	 "LotteryRebatePercentage" decimal, 
	 "CardGameRebatePercentage" decimal, 
	 "SportsRebatePercentage" decimal, 
	 "VirtualSportsRebatePercentage" decimal,
	 "TeamNumber" decimal,
	 "TotalCount" bigint) 
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 

	WITH RECURSIVE SUBAGENTS AS (
			SELECT A.ID, 
				   A.PARENT_AGENT_ID,
				   A.USERNAME,
				   CAST(USERNAME AS text) AS FULLNAME,
				   (SELECT COUNT(1) FROM DBO.PLAYERS WHERE AGENT_ID = A.ID) PLAYER_COUNT
			FROM DBO.AGENTS A
			WHERE A.BRAND_ID = _brand_id
		UNION 
			SELECT A.ID,
			       A.PARENT_AGENT_ID,
				   A.USERNAME,
			       CAST(S.FULLNAME || '->' || A.USERNAME AS text) AS FULLNAME,
			       (SELECT COUNT(1) FROM DBO.PLAYERS WHERE AGENT_ID = A.ID) PLAYER_COUNT
		    FROM DBO.AGENTS A
			INNER JOIN SUBAGENTS S ON S.PARENT_AGENT_ID = A.ID
			WHERE A.BRAND_ID = _brand_id )
SELECT A.ID as "Id",
	   A.BRAND_ID as "BrandId",
	   A.USERNAME as "Username",
	   A.STATUS as "Status",
	   A.AGENT_STATUS as "AgentStatus",
	   A.TYPE as "Type",
	   A.LEVEL_TYPE as "LevelType",
	   (SELECT BALANCE FROM DBO.AGENT_WALLETS WHERE AGENT_ID = A.ID) as "Balance",
	A.COMMISSION_PERCENTAGE as "CommissionPercentage",
	A.DIVIDEND_PERCENTAGE as "DividendPercentage",
	A.LIVE_DEALER_REBATE_PERCENTAGE as "LiveDealerRebatePercentage",
	A.SLOT_REBATE_PERCENTAGE as "SlotRebatePercentage",
	A.LOTTERY_REBATE_PERCENTAGE as "LotteryRebatePercentage",
	A.CARD_GAME_REBATE_PERCENTAGE as "CardGameRebatePercentage",
	A.SPORTS_REBATE_PERCENTAGE as "SportsRebatePercentage",
	A.VIRTUAL_SPORTS_REBATE_PERCENTAGE as "VirtualSportsRebatePercentage",
	T.AGENT_COUNT + T.PLAYER_COUNT as "TeamNumber",
	count(*) over() as "TotalCount"
FROM DBO.AGENTS A
INNER JOIN (SELECT ID,
				   COUNT(1) AGENT_COUNT,
				   SUM(PLAYER_COUNT) PLAYER_COUNT
			FROM SUBAGENTS
			GROUP BY ID) T ON A.ID = T.ID
WHERE A.BRAND_ID = _BRAND_ID
		AND (_STATUS IS NULL OR A.STATUS = _STATUS)
		AND (_TYPE IS NULL OR A.TYPE = _TYPE)
		AND (_LEVEL_TYPE IS NULL OR A.LEVEL_TYPE = _LEVEL_TYPE)
		AND ((_USERNAME IS NULL OR _USERNAME = '') OR A.USERNAME ilike '%' || _USERNAME || '%')
order by
	  case when _is_desc = false AND lower(_sort_by) = 'username'	then a.username	end asc,
	  case when _is_desc = false AND lower(_sort_by) = 'status'	then a.status	end asc,
	  case when _is_desc = true AND lower(_sort_by) = 'username'	then a.username	end desc,
	  case when _is_desc = true AND lower(_sort_by) = 'status'		then a.status		end desc
    offset (_page_number-1) * _page_size 
	limit _page_size;
	
END;
$$