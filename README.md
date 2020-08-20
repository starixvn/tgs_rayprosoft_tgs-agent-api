## Start Sql Server using Docker

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@#$" -p 1433:1433 --name sqlserver -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

## Migration Command 

Add-Migration NewMigrationName  -StartupProjectName DataServices

## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis


### How to run Nats

- Make sure Docker installed.
- Run the script in command line.

 ```
 docker run -d -p 4222:4222 -p 8222:8222 --name nats nats
```

- Run the script to trace and debug the server. Press CTRL + C to exit.

 ```
 docker run -p 4222:4222 -p 8222:8222 --name nats nats -DV
```

### Ports
You should only run one container once a time, or it will failed cause the port is already allocated.

- 4222  
Port for NATS server.
- 8222  
Port for monitoring.
You could access http://localhost:8222 in browser.

If you want to run more than one container once a time, please assign specific port.

 ```
docker run -d -p 4223:4223 -p 8223:8223 --name nats2 nats -p 4223 -m 8223
```

### How to remove
Run the script.

 ```
docker rm -f nats
```