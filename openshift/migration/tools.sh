#!/bin/bash

## Setup openshift tools

set -o errexit    # Exit when command fails
set -o nounset    # Exit when access undeclared
set -o pipefail   # Return last non-zero command
# set -o xtrace   # Tracing


parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

# ===================================================================================================
# Environment
# ---------------------------------------------------------------------------------------------------

export NAMESPACE_PREFIX=dfb30e
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="ocp4"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# ===================================================================================================
# Internal
# ---------------------------------------------------------------------------------------------------



# ===================================================================================================
# Jenkins
# ---------------------------------------------------------------------------------------------------
process_jenkins() {
  ## dotnet slave
  oc process -o=yaml \
    -f ../templates/jenkins-slave-dotnet.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## dotnet sonarqube slave
  oc process -o=yaml \
    -f ../templates/jenkins-slave-sonarqube-dotnet.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## zap slave
  oc process -o=yaml \
    -f ../templates/jenkins-slave-zap.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## selenium-maven slave
  oc process -o=yaml \
    -f ../templates/jenkins-slave-selenium-maven.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

}


# ===================================================================================================
# Sonarqube
# ---------------------------------------------------------------------------------------------------

process_sonarqube(){

  oc process -o=yaml \
    -f ../templates/sonarqube-postgresql.yaml \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

# **NOTE:** Plugins are lost when permanent storage is mounted on plugins directory. To reinstall plugins:
# cd /opt/sonarqube/extensions/plugins
# ZAP
# curl -sSL -o sonar-zap-plugin-1.2.0.jar https://github.com/Coveros/zap-sonar-plugin/releases/download/sonar-zap-plugin-1.2.0/sonar-zap-plugin-1.2.0.jar
# Then add csharp plugin from marketplace in the UI and restart sonarqube
}

# ===================================================================================================
# Selenium
# ---------------------------------------------------------------------------------------------------

# Using templates from https://github.com/akroon3r/selenium-openshift-templates
## Selenium Hub

process_selenium(){
  export SELENIUM_GIT_URL="https://raw.githubusercontent.com/akroon3r/selenium-openshift-templates/master"

  oc process -o=yaml \
    -f ${SELENIUM_GIT_URL}/selenium-hub.yaml \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Selenium Chrome Node
  oc process -o=yaml \
    -f ${SELENIUM_GIT_URL}/selenium-node-chrome.yaml \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

# **NOTE:** the node might have to be restarted if there are session timeout errors
}

# ===================================================================================================
# SearchAPI
# ---------------------------------------------------------------------------------------------------

process_searchapi(){
  ## Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="search-api"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/search-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/search-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}


  ## Scans Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/search-api-scans.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p apiDefinition=  \
    -p sonartoken=  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# DynAdapter
# ---------------------------------------------------------------------------------------------------
process_dynadapter(){
  ## Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="dynadapter"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/dynadapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/dynadapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Scans
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/dynadapter-scans.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p apiDefinition=  \
    -p sonartoken=  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# RequestAPI
# ---------------------------------------------------------------------------------------------------
process_requestapi(){
  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="request-api"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/request-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/request-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}


# ===================================================================================================
# Web/Rest Adapter
# ---------------------------------------------------------------------------------------------------
process_webrestadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Configuration evn/secrets
  ## Create secrets synchronized with Jenkins
  ## Option 1:
  ### Pass base64 value of private key.
  ### config.properties is taken from private-repo://test-automation/fams3-frontend-automation/src/main/java/com/fams3/qa/config/config.properties
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/config/selenium-maven-config.yaml \
    -p file=$(cat config.properties | base64 | tr -d '\n')  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Option 2:
  # oc create secret generic selenium-maven-config --from-file=filename=config.properties -n ${TOOLS_NAMESPACE}
  # oc label secret selenium-maven-config credential.sync.jenkins.openshift.io=true -n ${TOOLS_NAMESPACE}

  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}



# ===================================================================================================
# FAMS Request Inbound adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------
process_inboundadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/fams-request-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# Rest inbound adapter
# ---------------------------------------------------------------------------------------------------
process_inboundadapter2(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  ## Option 1:
  ### Pass base64 value of private key.
  ### config.properties is taken from private-repo://test-automation/fams3-frontend-automation/src/main/java/com/fams3/qa/config/config.properties
  cat config.properties | base64 | tr -d '\n'
  ### Copy the output and pass as argument for file
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/config/selenium-maven-config.yaml \
    -p file=  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Option 2:
  oc create secret generic selenium-maven-config --from-file=filename=config.properties
  oc label secret selenium-maven-config credential.sync.jenkins.openshift.io=true

  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# Web adapter
# ---------------------------------------------------------------------------------------------------

process_webadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  export DATAPARTNERSERVICE=
  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# File adapter
# ---------------------------------------------------------------------------------------------------

process_fileadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}


# ===================================================================================================
# FAMS Request Inbound adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------
process_iasearchadapter(){
  export APP_NAME="ia-search-web-adapter"

  # Image stream
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName=${APP_NAME}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/builds/ia-search-web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/ia-search-web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# FAMS Request Inbound adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------

process_unknown(){


  # Configuration evn/secrets
  ## Create secrets synchronized with Jenkins
  ## Option 1:
  ### Pass base64 value of private key.
  cat id_rsa | base64 | tr -d '\n'
  ### Copy the output and pass as argument for gitSshPrivateKey
  # oc process -o=yaml \
  #   -f ${GIT_URL}/openshift/templates/config/fams3-github-key.yaml \
  #   -p gitSshPrivateKey=  \
  #   | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Option 2:
  oc create secret generic fams3-github-key --from-file=ssh-privatekey=id_rsa --type=kubernetes.io/ssh-auth
  oc label secret fams3-github-key credential.sync.jenkins.openshift.io=true

  # Pipeline
  oc process -o=yaml \
    -f ${GIT_URL}/openshift/templates/builds/pipelines/web-rest-adapters-scans.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p apiDefinition=  \
    -p sonartoken=  \
    -p PI=  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

###############################################################################
# Main
###############################################################################

# _main()
#
# Usage:
#   _main [<options>] [<arguments>]
#
# Description:
#   Entry point for the program, handling basic option parsing and dispatching.

_main() {
  printf "Beginning tools build for %s" $NAMESPACE_PREFIX
#   process_jenkins()
#   process_sonarqube()
#   process_selenium()
#   process_searchapi()
#   process_requestapi()
#   process_inboundadapter()
#   process_inboundadapter2()
#   process_webadapter()
#   process_fileadapter()
#   process_iasearchadapter()
#   process_unknown()
# }

# Call `_main` after everything has been defined.
_main "$@"
