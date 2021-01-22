CREATE OR REPLACE PROCEDURE dbo.player_deauthorize_by_login_record_id(player_code varchar, login_record_id uuid)
LANGUAGE plpgsql
AS $$
begin

	update dbo.player_login_records plr
	set status = 1
	where plr.id = $2
	and plr.player_code = $1
	and plr.status  = 0
	and plr.expired_on >= now();

END;
$$