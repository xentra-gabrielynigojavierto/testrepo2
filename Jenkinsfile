pipeline {
agent any

environment {
    PATH = "/usr/bin/dotnet:${env.PATH}"
}

parameters {
    choice(name: 'ENV', choices: ['dev', 'staging', 'prod'], description: 'Choose environment')
    choice(name: 'SERVICE', choices: ['all', 'service-a', 'service-b', 'service-c'], description: 'Choose microservice')
}

stages {

    stage('Checkout') {
        steps {
            git branch: 'main', url: 'https://github.com/xentra-gabrielynigojavierto/testrepo2'
        }
    }


    stage('Select Services') {
        steps {
            script {
                // Map service names to directories and csproj files
                def serviceMap = [
                    'service-a': [folder: "service-a", csproj: "service-a.csproj"],
                    'service-b': [folder: "service-b", csproj: "service-b.csproj"],
                    'service-c': [folder: "service-c", csproj: "service-c.csproj"]
                ]

                if (params.SERVICE == 'all') {
                    services = serviceMap
                } else {
                    services = [(params.SERVICE): serviceMap[params.SERVICE]]
                }

                echo "Selected services: ${services.keySet()}"
            }
        }
    }

    stage('SonarQube Analysis') {
    environment {
        SONARQUBE_SERVER = 'http://3.15.149.51:9000'
        SONARQUBE_TOKEN  = credentials('sonarQube_token') // Jenkins credential ID
    }
    steps {
        withSonarQubeEnv('SonarQubeServer') {
            sh "dotnet sonarscanner begin /k:\"YourProjectKey\" /d:sonar.host.url=${SONARQUBE_SERVER} /d:sonar.login=${SONARQUBE_TOKEN}"
            sh "dotnet build"
            sh "dotnet sonarscanner end /d:sonar.login=${SONARQUBE_TOKEN}"
        }
    }
}


    stage('Build & Test') {
        steps {
            script {
                def parallelStages = [:]

                services.each { name, svc ->
                    parallelStages[name] = {
                        stage("Build & Test: ${name}") {
                            dir(svc.folder) {
                                sh "dotnet restore ${svc.csproj}"
                                sh "dotnet build ${svc.csproj} -c Release"
                                sh "dotnet test ${svc.csproj} -c Release"
                            }
                        }
                    }
                }

                parallel parallelStages
            }
        }
    }
}

post {
    always {
        echo "Pipeline finished."
    }
    success {
        echo "Pipeline succeeded!"
    }
    failure {
        echo "Pipeline failed!"
    }
}

}