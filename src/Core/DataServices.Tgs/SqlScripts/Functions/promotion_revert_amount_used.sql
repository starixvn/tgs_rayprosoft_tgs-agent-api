DROP FUNCTION IF EXISTS dbo.promotion_revert_amount_used;
CREATE OR REPLACE FUNCTION dbo.promotion_revert_amount_used(
	_promotion_id uuid, 
	_amount decimal)
RETURNS numeric
LANGUAGE plpgsql
AS $$
declare 
	row_count int;
	_amount_used decimal;
BEGIN
	
	set lock_timeout to 3000;
    
    update dbo.promotions
	set amount_used = amount_used - _amount
    where id = _promotion_id
	returning amount_used into _amount_used;

	get diagnostics row_count = ROW_COUNT;

	if row_count = 0 then
		raise exception 'promotion not found.' using ERRCODE = '5000';
	end if;
	
	return _amount_used;
	
END;
$$