--Apple Pay Initial Data
--1.dbo.payments
insert
	into
	dbo.payments (id,
	description,
	extend_data,
	status,
	code,
	"name",
	created_on,
	updated_on,
	created_by_username,
	updated_by_username)
values('14d139ca-951f-62d8-53ee-a4512f22e04b'::uuid,
'online payment platform',
null,
0,
'ApplePay',
'ApplePay',
now(),
now(),
'admin.enabled',
'admin.enabled');

--2.dbo.payment_methods
--bank_type:Bank

--3.dbo.payment_channels 
--Please check google document payment info for "Apple Pay", it only support few banks, not all banks.

--4.dbo.payment_method_channels
insert
	into
	dbo.payment_method_channels (id,
	payment_id,
	method_id,
	channel_id,
	status,
	created_on,
	updated_on,
	created_by_username,
	updated_by_username)
values('26cdccc5-6e40-59a3-6a43-0aec07554592'::uuid,
'14d139ca-951f-62d8-53ee-a4512f22e04b'::uuid,
'67252f3a-6826-45ce-8bbc-c53e2ca7cdcb'::uuid,
'76fd5896-b659-4d73-bfb8-d5eaf6ae71c3'::uuid,
0,
now(),
now(),
'admin.enabled',
'admin.enabled');

--5.brand_payments
--Random Guid: SELECT md5(random()::text || clock_timestamp()::text)::uuid
--insert
--	into
--	dbo.brand_payments (id,
--	brand_id,
--	display_order,
--	payment_method_channel_id,
--	transfer_type,
--	channel_name,
--	payment_status,
--	transfer_limit_from,
--	transfer_limit_to,
--	player_list_condition,
--	all_players,
--	identity_authentication,
--	phone_authentication,
--	deposited_amount_unlimited,
--	deposited_amount_min,
--	deposited_amount_max,
--	reason,
--	fee_percentage,
--	lowest_fee,
--	highest_fee,
--	created_on,
--	updated_on,
--	created_by_username,
--	updated_by_username,
--	extended_data)
--values('d9f4be76-2fad-0d4c-5ea4-235eee7185db'::uuid,
--'4e86719f-243f-46f6-b010-5c758b7285af'::uuid,
--1,
--'26cdccc5-6e40-59a3-6a43-0aec07554592'::uuid,
--0,
--'ApplePay_BankTransfer_Custom',
--0,
--100.000000,
--49999.000000,
--0,
--true,
--false,
--false,
--true,
--100.000000,
--49999.000000,
--'Reason',
--0.000000,
--0.000000,
--0.000000,
--now(),
--now(),
--'admin.enabled',
--'admin.enabled',
--'{"type": "3", "apiKey": "EXgNMxsIxfAFuupt9QQa", "storeGuid": "2FF00CD295A47E70B1CFFFACAC6D02CA"}'::jsonb::jsonb);
  
--6.dbo.player_bank_infos
--Please replace player_code,created_by_username,updated_by_username
-
--insert
--	into
--	dbo.player_bank_infos (id,
--	player_code,
--	brand_id,
--	payment_channel_id,
--	bank_type,
--	detail,
--	created_on,
--	updated_on,
--	created_by_username,
--	updated_by_username)
--values('bbe3bba6-7206-4666-9686-d5fc6be7d4f5'::uuid,
--'4ucmqplxb029',
--'4e86719f-243f-46f6-b010-5c758b7285af'::uuid,
--'76fd5896-b659-4d73-bfb8-d5eaf6ae71c3'::uuid,
--'Bank',
--'{"name": "nameNeedCHNorENG", "bankName": "bankName", "bankBranch": "111", "bankAccount": "111"}'::jsonb::jsonb,
--now(),
--now(),
--'4ucmqplxb029',
--'4ucmqplxb029');