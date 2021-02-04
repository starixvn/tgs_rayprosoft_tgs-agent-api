CREATE OR REPLACE FUNCTION dbo.brand_get_currencies_by_external_code(brand_id uuid)
RETURNS TABLE(
 "Key" varchar, 
 "Value" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct
    	bc.brand_currency_code as "Key",
    	bc.currency_code as "Value"    	
    from dbo.brand_currencies bc
    where bc.brand_id = $1;
END;
$$
