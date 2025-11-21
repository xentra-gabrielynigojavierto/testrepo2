pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                // Grab the latest code from GitHub
                checkout scm
            }
        }

        stage('Hello') {
            steps {
                // Just print a message
                echo "Hello! A push just happened at ${env.BUILD_URL}"
            }
        }

        stage('List Files') {
            steps {
                // Show the repo contents
                sh 'ls -la'
            }
        }

        stage('Create a File') {
            steps {
                // Create a file as an example action
                sh 'echo "Build ran at $(date)" > build-info.txt'
            }
        }
    }

    post {
        success {
            echo 'Pipeline finished successfully!'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
}
