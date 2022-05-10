DROP FUNCTION IF EXISTS dbo.agent_update_path;
CREATE OR REPLACE FUNCTION dbo.agent_update_path(
	_agent_id uuid,
	_agent_nlevel integer,
	_agent_path text,
	_new_agent_path text)
RETURNS void
LANGUAGE plpgsql
AS $$
BEGIN

   WITH agentPath AS(
	select id,nlevel(path) nlevel,path from dbo.agents where _agent_path::ltree @> path 
	and id != _agent_id
	)

    update dbo.agents
    set "path" = case when _new_agent_path is null 
	then subltree(ap.path, _agent_nlevel-1,nlevel)::ltree
	else concat(_new_agent_path,'.',subltree(ap.path, _agent_nlevel-1,nlevel))::ltree
	end
    from agentPath ap
	where dbo.agents.id = ap.id;
	
END;
$$

