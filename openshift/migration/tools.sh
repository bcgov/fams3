#!/bin/bash

## Setup openshift tools

set -o errexit    # Exit when command fails
set -o nounset    # Exit when access undeclared
set -o pipefail   # Return last non-zero command
# set -o xtrace   # Tracing

script_path=$(dirname "${BASH_SOURCE[0]}")
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

# ===================================================================================================
# Environment
# ---------------------------------------------------------------------------------------------------

export NAMESPACE_PREFIX=dfb30e
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="ocp4"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# Export env vars
export $(grep -v '^#' ${script_path}/.env | xargs)


export SEARCHAPI_URL="https://search-api-${NAMESPACE_PREFIX}-dev.apps.silver.devops.gov.bc.ca/swagger/v1/swagger.json"
export DYNADAPTER_URL="https://dynadapter-${NAMESPACE_PREFIX}-dev.apps.silver.devops.gov.bc.ca/swagger/v1/swagger.json"

# ===================================================================================================
# Internal
# ---------------------------------------------------------------------------------------------------

begin() {
   printf "Processing ${1}-----------------------------------------\n" 
}

finish() {
  printf "Done\n"
}

# ===================================================================================================
# Jenkins
# ---------------------------------------------------------------------------------------------------
process_jenkins() {
  ## dotnet slave
  begin jenkins-slave-dotnet
  oc process -o=yaml \
    -f ../templates/jenkins-slave-dotnet.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## dotnet sonarqube slave
  begin jenkins-slave-sonarqube-dotnet.yaml
  oc process -o=yaml \
    -f ../templates/jenkins-slave-sonarqube-dotnet.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## zap slave
  begin jenkins-slave-zap.yaml
  oc process -o=yaml \
    -f ../templates/jenkins-slave-zap.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## selenium-maven slave
  begin jenkins-slave-selenium-maven.yaml
  oc process -o=yaml \
    -f ../templates/jenkins-slave-selenium-maven.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}


# ===================================================================================================
# Sonarqube
# ---------------------------------------------------------------------------------------------------

process_sonarqube(){

  begin "sonarqube-postgresql"
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

  begin selenium-hub
  oc process -o=yaml \
    -f ${SELENIUM_GIT_URL}/selenium-hub.yaml \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Selenium Chrome Node
  begin selenium-node-chrome
  oc process -o=yaml \
    -f ${SELENIUM_GIT_URL}/selenium-node-chrome.yaml \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

# **NOTE:** the node might have to be restarted if there are session timeout errors
}

# ===================================================================================================
# SearchAPI
# ---------------------------------------------------------------------------------------------------

process_searchapi(){
  # Image stream
  begin build/generic
  oc process -o=yaml \
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="search-api"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  begin build/search-api
  oc process -o=yaml \
    -f ../templates/builds/builds/search-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  begin pipeline/search-api
  oc process -o=yaml \
    -f ../templates/builds/pipelines/search-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}


  ## Scans Pipeline
  begin pipeline/search-api-scans
  oc process -o=yaml \
    -f ../templates/builds/pipelines/search-api-scans.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p apiDefinition=${SEARCHAPI_URL}  \
    -p sonartoken=${SONAR_TOKEN} \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# DynAdapter
# ---------------------------------------------------------------------------------------------------
process_dynadapter(){
  ## Image stream
  begin build/generic
  oc process -o=yaml \
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="dynadapter"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  begin build/dynadapter
  oc process -o=yaml \
    -f ../templates/builds/builds/dynadapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  begin pipeline/dynadapter
  oc process -o=yaml \
    -f ../templates/builds/pipelines/dynadapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Scans
  begin pipeline/dynadapter-scans
  oc process -o=yaml \
    -f ../templates/builds/pipelines/dynadapter-scans.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p apiDefinition=${DYNADAPTER_URL}  \
    -p sonartoken=${SONAR_TOKEN}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# RequestAPI
# ---------------------------------------------------------------------------------------------------
process_requestapi(){
  # Image stream
  begin builds/generic
  oc process -o=yaml \
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName="request-api"  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Build config
  begin build/request-api
  oc process -o=yaml \
    -f ../templates/builds/builds/request-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Pipeline
  begin pipeline/request-api
  oc process -o=yaml \
    -f ../templates/builds/pipelines/request-api.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}


# ===================================================================================================
# Web/Rest Adapter
# ---------------------------------------------------------------------------------------------------
_process_rest(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/rest-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_web(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_fams_inbound(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/fams-request-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_rest_inbound(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/fams-request-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_file(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_ia_search(){
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName=${APP_NAME}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/ia-search-web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/ia-search-web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

process_adapters(){
  _process_rest bchydro
  _process_web bchydro
  _process_sonarscan bchydro
  
  _process_web cornet

  _process_rest_inbound fmep

  _process_rest icbc
  _process_web icbc
  
  _process_rest mhsd
  _process_web mhsd

  _process_rest moh-demo
  _process_web moh-demo
  _process_rest moh-emp
  _process_web moh-emp
  _process_web moh-rp
  _process_rest moh-rp

  _process_web wsbc
  _process_rest_inbound wsbc
  _process_file jca
  _process_ia_search
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
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/fams-request-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/rest-inbound-adapter.yaml \
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
    -f ../templates/config/selenium-maven-config.yaml \
    -p file=  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Option 2:
  oc create secret generic selenium-maven-config --from-file=filename=config.properties
  oc label secret selenium-maven-config credential.sync.jenkins.openshift.io=true

  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

# ===================================================================================================
# Web adapter
# ---------------------------------------------------------------------------------------------------

process_web(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  export DATAPARTNERSERVICE=
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/web-adapter.yaml \
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
    -f ../templates/builds/images/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/file-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/file-adapter.yaml \
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
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName=${APP_NAME}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/ia-search-web-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/ia-search-web-adapter.yaml \
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
  #   -f ../templates/config/fams3-github-key.yaml \
  #   -p gitSshPrivateKey=  \
  #   | oc apply -f - -n ${TOOLS_NAMESPACE}

  ## Option 2:
  oc create secret generic fams3-github-key --from-file=ssh-privatekey=id_rsa --type=kubernetes.io/ssh-auth
  oc label secret fams3-github-key credential.sync.jenkins.openshift.io=true

  # Pipeline
  oc process -o=yaml \
    -f ../templates/builds/pipelines/web-rest-adapters-scans.yaml \
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
  printf "Beginning tools build for %s\n" ${NAMESPACE_PREFIX}
#   process_jenkins           # Processed
#   process_sonarqube         # Processed
#   process_selenium          # Processed
#   process_searchapi         # Processed
#   process_dynadapter        # Processed
#   process_requestapi         # Processed

    process_adapters



#   process_inboundadapter
#   process_inboundadapter2
#   process_web
#   process_fileadapter
#   process_iasearchadapter
#   process_unknown
}

# Call `_main` after everything has been defined.
_main "$@"
