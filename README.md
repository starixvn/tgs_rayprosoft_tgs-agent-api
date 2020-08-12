-- Start Sql Server using Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@#$" -p 1433:1433 --name sqlserver  -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

