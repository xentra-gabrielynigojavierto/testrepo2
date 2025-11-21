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
        stage('Discover Microservices') {
            steps {
                script {
                    services = sh(
                        script: "find . -name '*.csproj' -exec dirname {} \\; | sed 's|./||' | sort -u",
                        returnStdout: true
                    ).trim().split("\n")

                    if (params.SERVICE != 'all') {
                        services = services.findAll { it == params.SERVICE }
                    }

                    echo "Detected services: ${services}"
                }
            }
        }

        stage('Build & Test') {
            steps {
                script {
                    def parallelStages = [:]

                    services.each { svc ->
                        parallelStages[svc] = {
                            stage("Build & Test: ${svc}") {
                                dir(svc) {
                                    sh "dotnet restore"
                                    sh "dotnet build -c Release"
                                    sh "dotnet test -c Release"
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
        always { echo "Pipeline finished." }
        success { echo "Pipeline succeeded!" }
        failure { echo "Pipeline failed!" }
    }
}
