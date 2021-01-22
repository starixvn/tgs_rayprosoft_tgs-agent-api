CREATE OR REPLACE FUNCTION dbo.brand_get_brand_game_provider_configuration_for_cache(brand_id uuid, game_provider_id uuid)
RETURNS TABLE(
    "DefaultLanguageCode" varchar,
    "DesktopQueryString" varchar,
    "MobileQueryString" varchar,
    "GameProviderConfigurationId" uuid)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select 
        default_language_code AS "DefaultLanguageCode",
        desktop_query_string AS "DesktopQueryString",
        mobile_query_string AS "MobileQueryString",
        game_provider_configuration_id AS "GameProviderConfigurationId"
    from dbo.brand_game_providers t
    where t.brand_id = $1
    and t.game_provider_id = $2;
END;
$$