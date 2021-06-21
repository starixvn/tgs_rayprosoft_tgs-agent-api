CREATE OR REPLACE FUNCTION dbo.promotion_revert_amount_used(
	_promotion_id uuid, 
	_amount decimal)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
	
	set lock_timeout to 3000;
	
	update dbo.promotions
	set amount_used = amount_used - _amount
	where id = _promotion_id;
	
	return true;
	
END;
$$