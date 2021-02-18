docker login registry.gitlab.com

@echo off
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%a%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME%") do (set mytime=%%a%%b)
SET datetime=%mydate%%mytime%
echo %datetime%
SET tag=dev-%datetime%

docker build -f Web\backoffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-site:%tag%

docker build -f Web\mocksite\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%tag%

docker build -f Apis\BackendOffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%tag%

docker build -f Apis\LogApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%tag%

docker build -f Apis\OperatorApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%tag%

docker build -f Apis\TgsApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%tag%

docker build -f Apis\FileService\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/file-service:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/file-service:%tag%

docker build -f Apis\GlobalApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/global-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/global-api:%tag%

docker build -f Worker\Schedulers\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/schedulers:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/schedulers:%tag%
