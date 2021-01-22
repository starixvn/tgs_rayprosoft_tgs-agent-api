CREATE OR REPLACE FUNCTION dbo.brand_get_languages_by_external_code(brand_id uuid)
RETURNS TABLE(
 "Key" varchar, 
 "Value" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct
    	bl.brand_culture_code as "Key",
    	bl.culture_code as "Value"
    from dbo.brand_languages bl
    where bl.brand_id = $1;
END;
$$
