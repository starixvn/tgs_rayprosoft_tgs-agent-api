docker login registry.gitlab.com

docker build -f Apis\BackendOffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:dev