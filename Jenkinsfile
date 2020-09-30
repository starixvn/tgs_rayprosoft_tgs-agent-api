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
                credentialsId: 'be635747-2b91-43ae-85a7-718952c64f43',
              ]]
             ])
           }
        }
    }
}
