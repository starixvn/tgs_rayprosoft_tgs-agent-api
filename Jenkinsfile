pipeline {
    agent any
    stages {
       stage('Checkout SCM') {
          steps {
            checkout([
              $class: 'GitSCM',
              branches: [[name: "**" ]],
              userRemoteConfigs: [[
                url: 'https://gitlab.com/rayprosoft/turnkey-gaming-system.git',
                credentialsId: '',
              ]]
             ])
           }
        }
        stage('Restore packages'){
            steps{
                sh "dotnet restore tgs.sln"
              }
        }
        stage('Build') {
            steps {
                sh '''
                echo "haha"
                '''
            }
        }
        stage('Test') {
            steps {
                sh '''
                echo "haha"
                '''
            }
        }
    }
}
