CREATE OR REPLACE FUNCTION dbo.promotion_update_amount_used(
    _PROMOTION_ID UUID,
    _AMOUNT decimal
)
RETURNS boolean
LANGUAGE PLPGSQL 
AS 
$$
DECLARE
    row_count int;
    _old_amount decimal;
    _new_amount decimal;
    _total_amount_limit decimal;
    _unlimited boolean;
BEGIN
    set lock_timeout to 3000;

    update dbo.promotions
    set 
        amount_used = amount_used + _amount
    where 
        id = _promotion_id
        AND
        (
			total_amount_limit > amount_used
			OR
			unlimited > 0
		)
    returning amount_used into _total_amount_limit;

    return _total_amount_limit IS NOT NULL;
END;
$$