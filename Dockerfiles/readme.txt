
docker login registry.gitlab.com

docker build -f Dockerfiles\Dockerfile.BackofficeApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev01 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev01


docker build -f Dockerfiles\Dockerfile.LogApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:dev01 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:dev01

docker build -f Dockerfiles\Dockerfile.OperatorApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:dev01 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:dev01

docker build -f Dockerfiles\Dockerfile.TgsApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:dev01 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:dev01
