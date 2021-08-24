docker login registry.gitlab.com

@echo off
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%a%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME%") do (set mytime=%%a%%b)
SET datetime=%mydate%%mytime%
echo %datetime%
SET tag=dev-%datetime%

docker build -f src\Web\mocksite\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/mocksite:%tag%

docker build -f src\Apis\BackendOffice\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/backoffice-api:%tag%

docker build -f src\Apis\LogApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/log-api:%tag%

docker build -f src\Apis\OperatorApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%tag% --secret id=nuget,src="%USERPROFILE%\AppData\Roaming\NuGet\NuGet.Config" .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/operator-api:%tag%

docker build -f src\Apis\TgsApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/tgs-api:%tag%

docker build -f src\Apis\GlobalApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/global-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/global-api:%tag%

docker build -f src\Apis\ReportApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/report-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/report-api:%tag%

docker build -f src\Apis\ValidateCodeApi\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/validatecode-api:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/validatecode-api:%tag%

docker build -f src\Workers\Schedulers\Dockerfile -t registry.gitlab.com/rayprosoft/turnkey-gaming-system/schedulers:%tag% .
docker push registry.gitlab.com/rayprosoft/turnkey-gaming-system/schedulers:%tag%
