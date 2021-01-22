CREATE OR REPLACE FUNCTION dbo.player_get_player_login_records(player_code varchar, statuses int[], "from" timestamptz)
RETURNS TABLE(
 "BrandId" uuid, 
 "PlatformType" smallint, 
 "CreatedOn" timestamptz , 
 "ExpiredOn" timestamptz)
LANGUAGE plpgsql
AS $$
begin

	RETURN QUERY		
	select
		plr.brand_id as "BrandId",
		plr.platform_type as "PlatformType",
		plr.created_on as "CreatedOn",
		plr.expired_on as "ExpiredOn"
	from dbo.player_login_records plr
	where plr.player_code  = $1
	and plr.created_on  >= $3
	and plr.status = any($2);

END;
$$
;
