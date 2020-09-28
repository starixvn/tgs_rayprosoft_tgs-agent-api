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
        stage('Test') {
            steps {
                sh '''
                echo "haha"
                '''
            }
        }
    }
}
