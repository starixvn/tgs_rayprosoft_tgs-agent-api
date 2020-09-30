pipeline {
    agent any
    stages {
       stage('Checkout SCM') {
          steps {
            checkout([
              $class: 'GitSCM',
              branches: [[name: "*/develop" ]],
              userRemoteConfigs: [[
                url: 'https://gitlab.com/rayprosoft/turnkey-gaming-system.git',
                credentialsId: 'gitlab_lester_account',
              ]]
             ])
           }
        }
    }
}
