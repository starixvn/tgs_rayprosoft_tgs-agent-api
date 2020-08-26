## Start Sql Server using Docker

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz2wsx!@#$" -p 1433:1433 --name sqlserver -d --restart unless-stopped  mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04

## Migration Command 

#TGS
add-migration <MigrationName> -c TgsRepository -p DataServices -s TgsApi

#Log
add-migration <MigrationName> -c LogRepository -p LogApi -s LogApi

#Backend Office
add-migration <MigrationName> -c BackendOfficeRepository -p BackendOffice -s BackendOffice


## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis

##Install Nats using Docker

docker run -d -p 4222:4222 -p 8222:8222 --name nats nats




#Get IP from comtainer
docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' comtainer_name


##Install argo 

kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

kubectl get pods -n argocd

kubectl port-forward svc/argocd-server -n argocd 8080:443

use git bash 

kubectl get pods -n argocd -l app.kubernetes.io/name=argocd-server -o name | cut -d'/' -f 2

argocd-server-75877b6ffb-zbm6k

