#!/bin/bash   
ProjectId=20478010
Token=oD2cksJhGN1tgUuaE6C9

for repositoriId in $(curl --header "PRIVATE-TOKEN: ${Token}" "https://gitlab.com/api/v4/projects/${ProjectId}/registry/repositories" | jq .[] | jq ."id"); do
	curl --request DELETE --data 'name_regex_delete=.*' --data 'keep_n=10' --header "PRIVATE-TOKEN: ${Token}" "https://gitlab.com/api/v4/projects/{ProjectId}/registry/repositories/${repositoriId}/tags"
done