
docker login registry.gitlab.com

# backoffice site's root path is under Web\backoffice
cd Web\backoffice
docker build -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:dev

# go to root path
cd ../..
docker build -f Dockerfiles\Dockerfile.BackOfficeApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev

docker build -f Dockerfiles\Dockerfile.LogApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:dev

docker build -f Dockerfiles\Dockerfile.OperatorApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:dev

docker build -f Dockerfiles\Dockerfile.TgsApi -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:dev
