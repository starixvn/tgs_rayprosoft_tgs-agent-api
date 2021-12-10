CREATE OR REPLACE FUNCTION DBO.BRAND_GAME_PROVIDERS_STATUS_CHANGES(_to_maintenance_text text, _to_active_text text) RETURNS VOID LANGUAGE PLPGSQL AS $$
declare
	to_maintenance_ids uuid[];
	to_active_ids uuid[];
BEGIN

    to_maintenance_ids = string_to_array(_to_maintenance_text,',');
	to_active_ids = string_to_array(_to_active_text,',');
	
    update dbo.brand_game_providers
    set status = 3 where id = ANY(to_maintenance_ids);

	update dbo.brand_game_providers
    set status = 0 where id = ANY(to_active_ids);

END;
$$