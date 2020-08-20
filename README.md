## Start Sql Server using Docker

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@#$" -p 1433:1433 --name sqlserver -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

## Migration Command 

Add-Migration NewMigrationName


## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis


