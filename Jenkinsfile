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
                sh "dotnet build tgs.sln"
                echo "build solution"
                '''
            }
        }
        stage('Test') {
            steps {
                sh  "dotnet test tgs.sln"  
                echo "test tgs.sln"
                '''
            }
        }
    }
}
