CREATE OR REPLACE FUNCTION dbo.game_provider_get_game_provider_configuration_for_cache(game_provider_configuration_id uuid)
RETURNS TABLE(
    "Id" uuid, 
    "Name" varchar, 
    "Description" varchar, 
    "Status" smallint, 
    "SecretKey" varchar, 
    "GameProviderId" uuid, 
    "DesktopEndpoint" varchar, 
    "MobileEndpoint" varchar, 
    "ConfigurationData" text, 
    "IsDefault" bool)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct
        gpc.id as "Id",
        gpc."name"  as "Name",
        gpc.description as "Description",
        gpc.status as "Status",
        gpc.secret_key as "SecretKey",
        gpc.game_provider_id as "GameProviderId",
        gpc.desktop_endpoint as "DesktopEndpoint",
        gpc.mobile_endpoint as "MobileEndpoint",
        gpc.configuration_data as "ConfigurationData",
        gpc.is_default as "IsDefault"
    from dbo.game_provider_configurations gpc 
    where gpc.id = $1;
END;
$$
;
