-- unique index of agents for lower case email, instead of alternative key
CREATE UNIQUE INDEX ux_agents_email
	ON dbo.agents (lower(email));