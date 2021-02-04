CREATE OR REPLACE PROCEDURE dbo.player_deauthorize_by_platform_type(player_code varchar, platform_type smallint)
LANGUAGE plpgsql
AS $$
begin

	update dbo.player_login_records plr
	set status = 1
	where plr.player_code = $1
	and (plr.platform_type = $2 or $2 is null)
	and plr.status = 0
	and plr.expired_on >= now();

END;
$$