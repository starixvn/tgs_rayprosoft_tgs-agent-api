UPDATE dbo.withdraw_forms AS wf
SET
	user_name = p.username,
	user_type = 1
FROM 
	dbo.players AS p
WHERE 
	p.player_code = wf.player_code;