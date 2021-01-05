#!/bin/bash   
ProjectId=20478010
Token=oD2cksJhGN1tgUuaE6C9
for row in $(curl --header "PRIVATE-TOKEN: ${Token}" "https://gitlab.com/api/v4/projects/${ProjectId}/registry/repositories" | jq .[] | jq ."id"); do
     echo ${row}
done