openshift.withCluster() {
  env.NAMESPACE = openshift.project()
  env.APP_NAME = "${JOB_NAME}".replaceAll(/-build.*/, '')
  echo "Starting Pipeline for ${APP_NAME}..."
  env.BUILD = "${env.NAMESPACE}"
  env.DEV = "${APP_NAME}-dev"
  env.STAGE = "${APP_NAME}-stage"
  env.PROD = "${APP_NAME}-prod"
}

podTemplate(containers: [
    containerTemplate(
      name: "jnlp",
      image: "registry.redhat.io/openshift3/jenkins-agent-maven-35-rhel7:v3.11",
    ),
    containerTemplate(name: 'dotnet', image: 'registry.redhat.io/dotnet/dotnet-22-rhel7:2.2', ttyEnabled: true, command: 'cat')
  ]) {

    node(POD_LABEL) {
        container('dotnet') {
            // Checkout source code
            // This is required as Pipeline code is originally checkedout to
            // Jenkins Master but this will also pull this same code to this slave
            stage('Git Checkout') {
                // Turn off Git's SSL cert check, uncomment if needed
                // sh 'git config --global http.sslVerify false'
                git url: "${APPLICATION_SOURCE_REPO}", branch: "${APPLICATION_SOURCE_REF}"
            }

            // Run Maven build, skipping tests
            stage('publish') {
                dir('web') {
                sh "dotnet publish -c Release /p:MicrosoftNETPlatformLibrary=Microsoft.NETCore.App"
                }
            }

            // Build Container Image using the artifacts produced in previous stages
            stage('Build Container Image'){
                // Build container image using local Openshift cluster
                // Giving all the artifacts to OpenShift Binary Build
                // This places your artifacts into right location inside your S2I image
                // if the S2I image supports it.
                binaryBuild(projectName: env.BUILD, buildConfigName: "${APP_NAME}-web", buildFromPath: "web/bin/Release/netcoreapp2.2/publish")
            }

            stage('Promote from Build to Dev') {
                tagImage(sourceImageName: "${APP_NAME}-web", sourceImagePath: env.BUILD, toImagePath: env.DEV)
            }

            stage ('Verify Deployment to Dev') {
                verifyDeployment(projectName: env.DEV, targetApp: "${APP_NAME}-web")
            }
        }
    }
}