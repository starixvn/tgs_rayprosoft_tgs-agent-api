CREATE OR REPLACE FUNCTION dbo.brand_get_brand_game_providers_for_cache(brand_id uuid)
RETURNS TABLE(
    "GameProviderId" uuid, 
    "GameProviderCode" varchar, 
    "GameProviderConfigurationId" uuid, 
    "BrandGameProviderStatus" smallint)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct 
        bgp.game_provider_id as "GameProviderId",
        gp.code as "GameProviderCode",
        bgp.game_provider_configuration_id as "GameProviderConfigurationId",
        bgp.status as "BrandGameProviderStatus"
    from dbo.game_providers as gp
    join dbo.brand_game_providers as bgp on bgp.game_provider_id = gp.id
    where bgp.brand_id = $1;
END;
$$
