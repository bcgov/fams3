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
export TAG=prod
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${TAG}
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
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
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
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
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
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
    -p gitRef=${GIT_BRANCH} \
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
    -p gitRef=${GIT_BRANCH} \
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
    -p gitRef=${GIT_BRANCH} \
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
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

}

_process_ia_search(){
  APP_NAME="ia-search-web-adapter"
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/generic.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p appName=${APP_NAME}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/ia-search-web-adapter.yaml \
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

}

_process_fmep_rest_inbound(){
  DATAPARTNERSERVICE=fmep
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/fams-request-inbound-adapter.yaml \
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

_process_wsbc_rest_inbound(){
  DATAPARTNERSERVICE=wsbc
  # Image stream
  oc process -o=yaml \
    -f ../templates/builds/images/rest-inbound-adapter.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}

  # Build config
  oc process -o=yaml \
    -f ../templates/builds/builds/rest-inbound-adapter.yaml \
    -p gitRef=${GIT_BRANCH} \
    -p namespacePrefix=${NAMESPACE_PREFIX}  \
    -p dataPartnerService=${DATAPARTNERSERVICE}  \
    | oc apply -f - -n ${TOOLS_NAMESPACE}
}

process_adapters(){
  _process_rest bchydro
  _process_web bchydro
  _process_web cornet
  _process_fmep_rest_inbound
  _process_wsbc_rest_inbound
  _process_rest icbc
  _process_web icbc
  _process_rest mhsd
  _process_web mhsd
  _process_rest moh-emp
  _process_web moh-emp
  _process_web moh-rp
  _process_rest moh-rp
  _process_web wsbc
  _process_rest wsbc
  _process_file jca
  _process_ia_search
}

_start_build(){
  if [ -z "$1" ]; then 
    exit
  fi
  BUILD=$1
  oc start-build $1 -n ${TOOLS_NAMESPACE} --wait
}

start_builds(){
  buildConfigs=(
    "bchydro-rest-adapter"
    "bchydro-web-adapter"
    "cornet-web-adapter"
    "dynadapter"
    "ia-search-web-adapter"
    "fmep-rest-inbound-adapter"
    "icbc-rest-adapter"
    "icbc-web-adapter"
    "jca-file-adapter"
    "mhsd-rest-adapter"
    "mhsd-web-adapter"
    "moh-emp-rest-adapter"
    "moh-emp-web-adapter"
    "moh-rp-rest-adapter"
    "moh-rp-web-adapter"
    "moh-demo-web-adapter"
    "moh-demo-rest-adapter"
    "request-api"
    "search-api"
    "wsbc-rest-inbound-adapter"
    "wsbc-web-adapter"
  )
  for bc in ${buildConfigs[@]}; do 
    _start_build $bc
  done

}

###############################################################################
# Deploy
###############################################################################

# ===================================================================================================
# RabbitMQ
# ---------------------------------------------------------------------------------------------------
deploy_rabbitmq(){
  begin rabbitmq
  oc process -o=yaml \
    -f ../templates/rabbitmq.dc.yaml \
    -p NAMESPACE=${TARGET_NAMESPACE} \
    | oc apply -f - -n ${TARGET_NAMESPACE}

    # TODO: Add password for prod
}

# ===================================================================================================
# Jaeger
# ---------------------------------------------------------------------------------------------------

deploy_jaeger(){
  begin jaeger
# All-in-one template
  oc process -o=yaml \
    -f ../templates/jaeger-aio.dc.yaml \
    | oc apply -f - -n ${TARGET_NAMESPACE}
}

# ===================================================================================================
# Redis
# ---------------------------------------------------------------------------------------------------

deploy_redis(){
  begin redis

  # for dev and test
  oc process -o=yaml \
    -f ../templates/redis/redis-persistent-commander.dc.yaml \
    -p namespacePrefix=${NAMESPACE_PREFIX} \
    -p TAG=${TAG}  \
    | oc apply -f - 

  # # for prod
  # oc process -o=yaml \
  #   -f ../templates/redis/redis-persistent.dc.yaml \
  #   -p namespacePrefix=${NAMESPACE_PREFIX} \
  #   -p TAG=${NAMESPACE_SUFFIX}  \
  #   | oc apply -f -
}

# ===================================================================================================
# SearchAPI
# ---------------------------------------------------------------------------------------------------

deploy_searchapi(){
  begin searchapi
  # Configuration evn/secrets
  #   -f ../templates/config/rabbit-mq-configuration.yaml 
  #   -f ../templates/config/aspnet-env.yaml 
  #   -f ../templates/config/jeager-config.yaml 
  #   -f ../templates/config/searchapi-webhook-config.yaml 
  #   -f ../templates/config/redis-connection.yaml 
  #   -f ../templates/config/splunk-config.yaml 

  oc process -f ../templates/applications/deployments/search-api.yaml \
                -p namespacePrefix=${NAMESPACE_PREFIX} \
                -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/search-api.yaml | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/routes/search-api.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
}

# ===================================================================================================
# DynAdapter
# ---------------------------------------------------------------------------------------------------
deploy_dynadapter(){
  begin dynadapter
  # Configuration evn/secrets
  #   -f ../templates/config/aspnet-env.yaml \
  #   -f ../templates/config/dynadapter-config.yaml \
  #   -f ../templates/config/dynamics-oauth-credentials.yaml \
  #   -f ../templates/config/jeager-config.yaml \
  #   -f ../templates/config/splunk-config.yaml \

  oc process -f ../templates/applications/deployments/dynadapter.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/services/dynadapter.yaml | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/routes/dynadapter.yaml \
                    -p namespacePrefix=${NAMESPACE_PREFIX} \
                    -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
}

# ===================================================================================================
# RequestAPI
# ---------------------------------------------------------------------------------------------------
deploy_requestapi(){
  begin requestapi
  # -f ../templates/config/requestapi-webhook-config.yaml \

  oc process -f ../templates/applications/deployments/request-api.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/services/request-api.yaml | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/routes/request-api.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

}


# ===================================================================================================
# IA Search Adapter
# ---------------------------------------------------------------------------------------------------
deploy_iasearchadapter(){
  begin iasearchadapter
  # -f ../templates/config/requestapi-webhook-config.yaml \

  oc process -f ../templates/applications/deployments/ia-search-web-adapter.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/services/ia-search-web-adapter.yaml | oc apply -f - -n ${TARGET_NAMESPACE}
  oc process -f ../templates/applications/routes/ia-search-web-adapter.yaml \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

}

# ===================================================================================================
# RestInboundAdapter
# ---------------------------------------------------------------------------------------------------
deploy_fmeprestinboundadapter(){
  DATAPARTNERSERVICE=fmep

  oc process -f ../templates/applications/deployments/fmep-rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} | oc apply -f - -n ${TARGET_NAMESPACE}
  
  oc process -f ../templates/applications/routes/rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  
}


# ===================================================================================================
# WebAdapter
# ---------------------------------------------------------------------------------------------------
deploy_webadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  oc process -f ../templates/applications/deployments/web-adapter.yaml \
                -p dataPartnerService=${DATAPARTNERSERVICE} \
                -p namespacePrefix=${NAMESPACE_PREFIX} \
                -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/web-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} | oc apply -f - -n ${TARGET_NAMESPACE}
  
  oc process -f ../templates/applications/routes/web-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  
}

# ===================================================================================================
# RestAdapter
# ---------------------------------------------------------------------------------------------------
deploy_restadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  oc process -f ../templates/applications/deployments/rest-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/rest-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} | oc apply -f - -n ${TARGET_NAMESPACE}
  
  oc process -f ../templates/applications/routes/rest-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  
}

# ===================================================================================================
# RestInboundAdapter
# ---------------------------------------------------------------------------------------------------
deploy_restinboundadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  oc process -f ../templates/applications/deployments/rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} | oc apply -f - -n ${TARGET_NAMESPACE}
  
  oc process -f ../templates/applications/routes/rest-inbound-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
  
}

# ===================================================================================================
# FileAdapter
# ---------------------------------------------------------------------------------------------------
deploy_fileadapter(){
  if [ -z "$1" ]; then 
    exit
  fi
  DATAPARTNERSERVICE=$1

  oc process -f ../templates/applications/deployments/file-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}

  oc process -f ../templates/applications/services/file-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} | oc apply -f - -n ${TARGET_NAMESPACE}
  
  oc process -f ../templates/applications/routes/file-adapter.yaml \
                  -p dataPartnerService=${DATAPARTNERSERVICE} \
                  -p namespacePrefix=${NAMESPACE_PREFIX} \
                  -p TAG=${TAG} | oc apply -f - -n ${TARGET_NAMESPACE}
}

# ===================================================================================================
# FileAdapter
# ---------------------------------------------------------------------------------------------------
_tag_image(){
  IMAGE=$1
  ENV=$TAG

  oc tag ${IMAGE}:latest ${IMAGE}:${ENV} -n ${TOOLS_NAMESPACE}

}

tag_images(){
  _tag_image dynadapter
  _tag_image search-api
  _tag_image request-api
  _tag_image ia-search-web-adapter
  _tag_image fmep-rest-inbound-adapter
  _tag_image icbc-rest-adapter
  _tag_image icbc-web-adapter
  _tag_image jca-file-adapter
  _tag_image mhsd-rest-adapter
  _tag_image mhsd-web-adapter
  _tag_image moh-demo-rest-adapter
  _tag_image moh-demo-web-adapter
  _tag_image moh-emp-rest-adapter
  _tag_image moh-emp-web-adapter
  _tag_image moh-rp-rest-adapter
  _tag_image moh-rp-web-adapter
  _tag_image bchydro-rest-adapter 
  _tag_image bchydro-web-adapter
  _tag_image cornet-web-adapter
  _tag_image wsbc-rest-inbound-adapter
  _tag_image wsbc-rest-adapter
  _tag_image wsbc-web-adapter
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
  process_searchapi 
  process_dynadapter  
  process_requestapi
  process_adapters
  start_builds

  deploy_jaeger
  deploy_redis
  deploy_requestapi
  deploy_searchapi
  deploy_dynadapter

  deploy_iasearchadapter
  deploy_fmeprestinboundadapter

  deploy_webadapter bchydro
  deploy_restadapter bchydro
  deploy_webadapter cornet
  deploy_webadapter icbc
  deploy_restadapter icbc
  deploy_fileadapter jca
  deploy_restadapter mhsd
  deploy_webadapter mhsd
  deploy_restadapter moh-emp
  deploy_webadapter moh-emp
  deploy_restadapter moh-rp
  deploy_webadapter moh-rp
  deploy_restadapter moh-demo
  deploy_webadapter moh-demo
  deploy_restadapter wsbc
  deploy_webadapter wsbc
  deploy_restinboundadapter wsbc

  tag_images 
}

# Call `_main` after everything has been defined.
_main "$@"
