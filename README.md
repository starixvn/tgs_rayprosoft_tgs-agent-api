## Start Sql Server using Docker

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@#$" -p 1433:1433 --name sqlserver -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

## Migration Command 

#TGS
add-migration NewMigrationName -c TgsRepository -p DataServices -s TgsApi

#Log
add-migration NewMigrationName -c LogDbContext -p LogApi -s LogApi


## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis

##Install Nats using Docker

docker run -d -p 4222:4222 -p 8222:8222 --name nats nats