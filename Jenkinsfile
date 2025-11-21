pipeline {
    agent any

    environment {
        AWS_CREDS        = credentials('aws-jenkins-creds')
        SONAR_SERVER     = 'sonarqube-server'
        SONAR_AUTH_TOKEN = credentials('sonarQube_token')
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

        stage('Discover Microservices') {
            steps {
                script {
                    // Find all directories containing a .csproj file
                    services = sh(
                        script: "find . -name '*.csproj' -exec dirname {} \\; | sed 's|./||' | sort -u",
                        returnStdout: true
                    ).trim().split("\n")

                    // Filter if user selected a specific service
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

        stage('SonarQube Scan') {
            steps {
                script {
                    services.each { svc ->
                        dir(svc) {
                            withSonarQubeEnv("${SONAR_SERVER}") {
                                sh """
                                    dotnet sonarscanner begin /k:"${svc}" /d:sonar.login="${env.SONAR_AUTH_TOKEN}" /d:sonar.host.url="${SONAR_SERVER}"
                                    dotnet build -c Release
                                    dotnet sonarscanner end /d:sonar.login="${env.SONAR_AUTH_TOKEN}"
                                """
                            }
                        }
                    }
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
