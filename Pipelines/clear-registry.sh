#!/bin/bash   
ProjectId=20478010
Token=oD2cksJhGN1tgUuaE6C9

for repositoriId in $(curl --header "PRIVATE-TOKEN: ${Token}" "https://gitlab.com/api/v4/projects/${ProjectId}/registry/repositories" | jq .[] | jq ."id"); do
	curl --request DELETE --data 'name_regex_delete=.*' --data 'keep_n=10' --header "PRIVATE-TOKEN: ${Token}" "https://gitlab.com/api/v4/projects/{ProjectId}/registry/repositories/${repositoriId}/tags"
done


//** Get Tag
//curl --header "PRIVATE-TOKEN: oD2cksJhGN1tgUuaE6C9" "https://gitlab.com/api/v4/projects/20478010/registry/repositories/1355692/tags?search=dev" | jq .[] | jq ."name"

//** Get Tag using filter
//curl --header "PRIVATE-TOKEN: oD2cksJhGN1tgUuaE6C9" "https://gitlab.com/api/v4/projects/20478010/registry/repositories/1355692/tags?name=dev$" | jq -c '.[] | select(.name | contains("dev"))'

// dos2unix clear-registry.sh