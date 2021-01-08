docker login registry.gitlab.com

docker build -f Web\backoffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:dev .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:dev