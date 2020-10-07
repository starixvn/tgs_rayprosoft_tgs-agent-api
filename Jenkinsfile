pipeline {
    agent {
	    docker {
			image: 'mcr.microsoft.com/dotnet/sdk:5.0' 
		}
	}
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
		stage('Restore packages'){
			steps{
				sh "dotnet restore tgs.sln"
				echo "restore tgs.sln"
			  }
		}
		stage('Build') {
			steps {
				sh "dotnet build tgs.sln"
				echo "build solution"
			}
		}
		stage('Test') {
			steps {
				sh  "dotnet test tgs.sln"  
				dotcover()
				echo "test tgs.sln"
			}
		}
    }
}
