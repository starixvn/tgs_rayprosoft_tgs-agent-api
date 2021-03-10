## Migration Command 

#TGS
add-migration <MigrationName> -c TgsRepository -p DataServices.Tgs -s TgsApi

#Log
add-migration <MigrationName> -c LogRepository -p DataServices.Log -s LogApi

#Report
add-migration <MigrationName> -c ReportRepository -p DataServices.Report -s ReportApi


## Install Redis using Docker

docker pull redis

docker run --name redis-tgs -p 6379:6379 -d redis

## Install Nats using Docker

docker run -d -p 4222:4222 -p 8222:8222 --name nats nats


# Get IP from comtainer
docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' container_name

docker network inspect bridge

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

## StartUp docker compose

stop to convert to windows style when git pull branch
1. git config --global core.autocrlf false
#reference https://docs.github.com/en/github/using-git/configuring-git-to-handle-line-endings#global-settings-for-line-endings



