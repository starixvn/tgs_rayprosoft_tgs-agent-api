docker login registry.gitlab.com

cd Web\backoffice
docker build -f Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:%1

cd Web\mocksite
docker build -f Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%1

cd ../..
docker build -f Apis\BackendOffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%1

docker build -f Apis\LogApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%1

docker build -f Apis\OperatorApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%1

docker build -f Apis\TgsApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%1

docker build -f Apis\FileService\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/file-service:%1 .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/file-service:%1
