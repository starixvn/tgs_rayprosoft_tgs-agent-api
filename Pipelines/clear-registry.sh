#!/bin/bash   
ProjectId=20478010
for row in $(curl --header "PRIVATE-TOKEN: oD2cksJhGN1tgUuaE6C9" "https://gitlab.com/api/v4/projects/${ProjectId}/registry/repositories" | jq .[] | jq ."id"); do
    _jq() {
     echo ${row} | jq -r ${1}
    }
done