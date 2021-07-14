def deployedComponents = ""

pipeline {
    agent any
	
	environment {
		Registry = "registry.gitlab.com/rayprosoft/turnkey-gaming-system/"
        BuildConfiguration='Debug'
	}

    stages {
        stage('Prepare') {
            steps {
                sh "echo 'Process components: $Components'";
                sh "echo 'Branch: $GIT_BRANCH'";
                script {
                    switch(env.GIT_BRANCH)
                    {
                        case 'origin/master':
                            env.kube_ns = "prod";
                            break;
                        case 'origin/release':
                            env.kube_ns = "staging";
                            break;
                        default:
                            env.kube_ns = "dev";
                            break;
                    }
                }

                configFileProvider([
					configFile(
						fileId: 'tgs.' + env.kube_ns , 
						variable: 'tgsconfig')]) {
					script {
						def envItems = readProperties file: "$tgsconfig";

                        envItems.each { key, value -> 
                            env."tgs_${key}" = value;
                        }

                        env.TAG = env.kube_ns + '-' + sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim()
					}
				}

                sh "echo tag: $TAG"
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -c $BuildConfiguration'
            }
        }

        stage('Test') {
			steps {
				sh  "dotnet test -c $BuildConfiguration --filter Unit /p:CollectCoverage=true --logger trx --no-build"
			}
		}

		stage('Building op-api image') {
            when {
                expression {
                    return checkIsRunStage("operator-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}operator-api"
            }
			steps {
				sh 'dotnet build "src/Apis/OperatorApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building agent-api image') {
            when {
                expression {
                    return checkIsRunStage("agent-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}agent-api"
            }
			steps {
                sh 'dotnet build "src/Apis/AgentApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building backoffice-api image') {
            when {
                expression {
                    return checkIsRunStage("backoffice-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}backoffice-api"
            }
			steps {
                sh 'dotnet build "src/Apis/BackendOffice" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building global-api image') {
            when {
                expression {
                    return checkIsRunStage("global-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}global-api"
            }
			steps {
                sh 'dotnet build "src/Apis/GlobalApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building log-api image') {
            when {
                expression {
                    return checkIsRunStage("log-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}log-api"
            }
			steps {
                sh 'dotnet build "src/Apis/LogApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building report-api image') {
            when {
                expression {
                    return checkIsRunStage("report-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}report-api"
            }
			steps {
                sh 'dotnet build "src/Apis/ReportApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building validatecode-api image') {
            when {
                expression {
                    return checkIsRunStage("validatecode-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}validatecode-api"
            }
			steps {
                sh 'dotnet build "src/Apis/ValidateCodeApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building promotion-api image') {
            when {
                expression {
                    return checkIsRunStage("promotion-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}promotion-api"
            }
			steps {
                sh 'dotnet build "src/Apis/PromotionApi" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building file-service image') {
            when {
                expression {
                    return checkIsRunStage("file-service", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}file-service"
            }
			steps {
                sh 'dotnet build "src/Apis/FileService" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Building schedulers image') {
            when {
                expression {
                    return checkIsRunStage("schedulers", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}schedulers"
            }
			steps {
                sh 'dotnet build "src/Workers/Schedulers" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Deploy') {
            when {
                expression {
                    return env.kube_ns != 'dev';
                }
            }
			input {
				message 'Ready to go?'
				ok 'yes'
			}

            steps {
                sh "echo start deploy"
            }
        }

        stage('Deploy op-api') {
            when {
                expression {
                    return checkIsRunStage("operator-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}operator-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template 'deployment/k8s/operator-api' \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set validateCodeTestEnabled=${tgs_validateCodeTestEnabled} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
			}
        }

        stage('Deploy agent-api') {
            when {
                expression {
                    return checkIsRunStage("agent-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}agent-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/agent-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy backoffice-api') {
            when {
                expression {
                    return checkIsRunStage("backoffice-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}backoffice-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/backoffice-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy global-api') {
            when {
                expression {
                    return checkIsRunStage("global-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}global-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/global-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy log-api') {
            when {
                expression {
                    return checkIsRunStage("log-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}log-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/log-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy report-api') {
            when {
                expression {
                    return checkIsRunStage("report-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}report-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/report-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy validatecode-api') {
            when {
                expression {
                    return checkIsRunStage("validatecode-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}validatecode-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/validatecode-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy promotion-api') {
            when {
                expression {
                    return checkIsRunStage("promotion-api", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}promotion-api"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/promotion-api" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy file-service') {
            when {
                expression {
                    return checkIsRunStage("file-service", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}file-service"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/file-service" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.mongo=${tgs_mongo}" \
                            --set "nats.urls=${tgs_nats}" \
                            --set "aws.accessKey=${tgs_aws_access_key}" \
                            --set "aws.Secret=${tgs_aws_access_secret}" \
                            --set "aws.region=${tgs_aws_region}" \
                            --set "aws.public.externalUrl=${tgs_file_public_external}" \
                            --set "aws.public.serviceUrl=${file_public_service}" \
                            --set "aws.public.bucket=${tgs_file_public_bucket}" \
                            --set "aws.private.externalUrl=${tgs_file_private_external}" \
                            --set "aws.private.serviceUrl=${tgs_file_private_service}" \
                            --set "aws.private.bucket=${tgs_file_private_bucket}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }

        stage('Deploy schedulers') {
            when {
                expression {
                    return checkIsRunStage("schedulers", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}schedulers"
            }
            steps {
                withKubeConfig([credentialsId: 'k8s-kube-config']) {
                    sh '''
                        helm template "deployment/k8s/schedulers" \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set "connectionStrings.tgs=${tgs_tgs}" \
                            --set "connectionStrings.redis=${tgs_redis}" \
                            --set "connectionStrings.mongo=${tgs_mongo}" \
                            --set "nats.urls=${tgs_nats}" \
                        | tee /dev/stdout \
                        | kubectl apply -f -
                    '''
                }
            }
        }
	}
	
	 post {
        success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK!\nDeployed Components: ${deployedComponents} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} \n Deploy Components: ${deployedComponents} \n Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
    }
}

def checkIsRunStage(currentName, selectedNames) {
    def arr = selectedNames.split(",")
    def isRun = false

    if(arr.length > 0)
        for(i = 0; i < arr.length; i++) {
            isRun = isRun || (arr[i] == currentName)
        }

    return isRun
}

def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")

    withCredentials([string(credentialsId: "telegramToken", variable: "TOKEN"), string(credentialsId: "telegramChatId", variable: "CHAT_ID")]) {

        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: "https://api.telegram.org/bot${TOKEN}/sendMessage?text=${encodedMessage}&chat_id=${CHAT_ID}",
                validResponseCodes: '200')
        return response
    }
}

def sendTelegramHTML(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")

    withCredentials([string(credentialsId: "telegramToken", variable: "TOKEN"), string(credentialsId: "telegramChatId", variable: "CHAT_ID")]) {

        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'POST',
                requestBody: "parse_mode=HTML&text=${encodedMessage}&chat_id=${CHAT_ID}",
                url: "https://api.telegram.org/bot${TOKEN}/sendMessage",
                validResponseCodes: '200')
        return response
    }
}
