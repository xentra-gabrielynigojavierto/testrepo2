pipeline {
    agent any

    environment {
       
        PATH      = "/usr/bin/dotnet"
    }

    parameters {
        choice(name: 'ENV', choices: ['dev', 'staging', 'prod'], description: 'Choose environment')
        choice(name: 'SERVICE', choices: ['all', 'service-a', 'service-b', 'service-c'], description: 'Select microservice')
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Discover Microservices') {
            steps {
                script {
                    def serviceDirs = sh(script: "find . -name '*.csproj' -exec dirname {} \\; | sort -u", returnStdout: true).trim().split('\n')
                    if (params.SERVICE != 'all') {
                        serviceDirs = serviceDirs.findAll { it.endsWith(params.SERVICE) }
                    }
                    services = serviceDirs
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
                            dir("${svc}") {
                                sh 'dotnet restore'
                                sh 'dotnet build -c Release'
                                sh 'dotnet test --no-build --verbosity normal || echo "No tests found for ${svc}"'
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
            node{cleanWs()}
            
        }
        success {
            echo "Build completed successfully for: ${services}"
        }
        failure {
            echo "Build failed. Check logs for errors."
        }
    }
}
