DROP FUNCTION IF EXISTS DBO.AGENT_GET_ALL_SUB_AGENTS;
CREATE OR REPLACE FUNCTION DBO.AGENT_GET_ALL_SUB_AGENTS(_AGENT_PATH text) 
RETURNS TABLE ("Id" UUID,"Username" CHARACTER varying, "ParentAgentId" UUID,"Path" ltree) 
LANGUAGE PLPGSQL AS 
$$
BEGIN
    RETURN QUERY

	select id "Id",
	username "Username",
	parent_agent_id "ParentAgentId",
	path "Path"
	from dbo.agents where _agent_path::ltree @> path;

END;
$$