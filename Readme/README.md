## Start Sql Server using Docker

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@" -p 1433:1433 --name sqlserver -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

## Migration Command 

#TGS
add-migration <MigrationName> -c TgsRepository -p DataServices -s TgsApi

#Bet
add-migration <MigrationName> -c BetRepository -p  DataServices.Bet -s TgsApi

#Log
add-migration <MigrationName> -c LogRepository -p DataServices.Log -s LogApi

#Backend Office
add-migration <MigrationName> -c BackendOfficeRepository -p BackendOffice -s BackendOffice


## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis

##Install Nats using Docker

docker run -d -p 4222:4222 -p 8222:8222 --name nats nats




#Get IP from comtainer
docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' container_name

docker network inspect bridge



##Install argo 

kubectl create namespace argocd

kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

kubectl get pods -n argocd

kubectl port-forward svc/argocd-server -n argocd 8080:443

use git bash 

kubectl get pods -n argocd -l app.kubernetes.io/name=argocd-server -o name | cut -d'/' -f 2

argocd-server-75877b6ffb-zbm6k



## docker push to gitlab

docker run --name dev-backoffice-api -p 8001:8001 registry.gitlab.com/rayprosoft/turnkey-gaming-system/dev-backoffice-api

docker run -dt -v "C:\Users\lesterwei\vsdbg\vs2017u5:/remote_debugger:rw" -v "D:\Code\tgs\Apis\BackendOffice:/app" -v "D:\Code\tgs:/src/" -v "C:\Users\lesterwei\AppData\Roaming\Microsoft\UserSecrets:/root/.microsoft/usersecrets:ro" -v "C:\Users\lesterwei\AppData\Roaming\ASP.NET\Https:/root/.aspnet/https:ro" -v "C:\Users\lesterwei\.nuget\packages\:/root/.nuget/fallbackpackages" -e "DOTNET_USE_POLLING_FILE_WATCHER=1" -e "ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true" -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=https://+:443;http://+:80" -e "NUGET_PACKAGES=/root/.nuget/fallbackpackages" -e "NUGET_FALLBACK_PACKAGES=/root/.nuget/fallbackpackages" -P --name BackendOffice --entrypoint tail tgsapisbackendoffice:dev -f /dev/null 



## Install mongoDb

startup mongoDb docker-compose



## Install localstack

1. install AWS CLI
https://aws.amazon.com/tw/cli/

2. startup localstack docker-compose

3. create a bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://dev-files

4.visit the Web UI
http://localhost:8080/




