CREATE OR REPLACE FUNCTION dbo.brand_get_brand_for_cache(brand_id uuid)
RETURNS TABLE(
	 "Id" uuid, 
	 "Code" varchar, 
	 "Name" varchar, 
	 "TimeZoneId" varchar, 
	 "Status" smallint, 
	 "ClientId" varchar, 
	 "SecretKey" varchar, 
	 "LicenseeId" uuid, 
	 "LicenseeCode" varchar, 
	 "ResellerId" uuid, 
	 "ResellerCode" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
	
	RETURN QUERY	
	with brand as
	(
		select 			
			id,
			time_zone_id,
			code,
			"name",
			status,
			client_id,
			secret_key,
			licensee_id
		from dbo.brands
		where id = $1
	)
	select
		b.id as "Id",
		b.code as "Code",
		b."name" as "Name",
		b.time_zone_id as "TimeZoneId",
		b.status as "Status",		
		b.client_id as "ClientId",
		b.secret_key as "SecretKey",
		l.id AS "LicenseeId",
		l.code AS "LicenseeCode",
		r.id AS "ResellerId",
		r.code AS "ResellerCode"		
	from brand b
	join dbo.licensees l on l.id = b.licensee_id
	join dbo.resellers r on r.id = l.reseller_id;
END;
$$
