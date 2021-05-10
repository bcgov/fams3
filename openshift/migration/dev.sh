#!/bin/bash

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

# Export env vars
export $(grep -v '^#' ${BASH_SOURCE[0]}/.env | xargs)

export NAMESPACE_PREFIX=dfb30e
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="ocp4"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# ===================================================================================================
# Selenium
# ---------------------------------------------------------------------------------------------------

# Static web server (for reports)
# Need access to create service account and role binding in target namespace
export SELENIUM_GIT_URL="https://raw.githubusercontent.com/akroon3r/selenium-openshift-templates/master"

oc process -o=yaml \
  -f ${SELENIUM_GIT_URL}/openshift/templates/static-web-server.dc.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX} \
  | oc apply -f - ${TARGET_NAMESPACE}


# ===================================================================================================
# RabbitMQ
# ---------------------------------------------------------------------------------------------------

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/rabbitmq.dc.yaml \
  | oc apply -f - ${TARGET_NAMESPACE}

# ===================================================================================================
# Jaeger
# ---------------------------------------------------------------------------------------------------

# All-in-one template
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/jaeger-aio.dc.yaml \
  | oc apply -f - ${TARGET_NAMESPACE}


# ===================================================================================================
# Redis
# ---------------------------------------------------------------------------------------------------

# for dev and test
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/redis/redis-persistent-commander.dc.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX} \
  -p TAG=${NAMESPACE_SUFFIX}  \
  | oc apply -f -

# for prod
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/redis/redis-persistent.dc.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX} \
  -p TAG=${NAMESPACE_SUFFIX}  \
  | oc apply -f -

# ===================================================================================================
# SearchAPI
# ---------------------------------------------------------------------------------------------------

# Configuration evn/secrets
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rabbit-mq-configuration.yaml \
  -p HOST=  \
  -p PORT=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/searchapi-webhook-config.yaml \
  -p NAME=  \
  -p URL=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/redis-connection.yaml \
  -p HOST=  \
  -p PORT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/splunk-config.yaml \
  -p URL=  \
  -p TOKEN=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

# ===================================================================================================
# DynAdapter
# ---------------------------------------------------------------------------------------------------
# Configuration evn/secrets
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/dynadapter-config.yaml \
  -p CRON=  \
  -p URL=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/dynamics-oauth-credentials.yaml \
  -p APIGATEWAY=  \
  -p CLIENTID=  \
  -p OAUTHURL=  \
  -p PASSWORD=  \
  -p RESOURCEURL=  \
  -p SECRET=  \
  -p USERNAME=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/splunk-config.yaml \
  -p URL=  \
  -p TOKEN=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}


# ===================================================================================================
# RequestAPI
# ---------------------------------------------------------------------------------------------------
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/requestapi-webhook-config.yaml \
  -p NAME=  \
  -p URL=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

# ===================================================================================================
# Web/Rest Adapter
# ---------------------------------------------------------------------------------------------------
export DATAPARTNERSERVICE=

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rest-config.yaml \
  -p APP_NAME=  \
  -p URL=  \
  -p CODE=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

# ===================================================================================================
# FAMS Request Inbound adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------

export DATAPARTNERSERVICE=


oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}


oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rest-inbound-config.yaml \
  -p APP_NAME=  \
  -p PROFILE_NAME=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}


# ===================================================================================================
# Rest Inbound adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------

export DATAPARTNERSERVICE=

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rest-inbound-config.yaml \
  -p APP_NAME=  \
  -p PROFILE_NAME=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}


# ===================================================================================================
# Web adapter deployment pipeline
# ---------------------------------------------------------------------------------------------------
export DATAPARTNERSERVICE=
# Image stream
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rabbit-mq-configuration.yaml \
  -p HOST=  \
  -p PORT=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/web-adapter-config.yaml \
  -p APP_NAME=  \
  -p URL=  \
  -p PROFILE_NAME=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/splunk-config.yaml \
  -p URL=  \
  -p TOKEN=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}


# ===================================================================================================
# JCA Adapter Deployment Pipeline
# ---------------------------------------------------------------------------------------------------
export DATAPARTNERSERVICE=

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/rabbit-mq-configuration.yaml \
  -p HOST=  \
  -p PORT=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/file-adapter-config.yaml \
  -p APP_NAME=  \
  -p URL=  \
  -p PROFILE_NAME=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/splunk-config.yaml \
  -p URL=  \
  -p TOKEN=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}

# ===================================================================================================
#  IA Search Adapter Deployment Pipeline
# ---------------------------------------------------------------------------------------------------
export APP_NAME="ia-search-web-adapter"

# Configuration evn/secrets
oc process -o=yaml  \
   -f ${GIT_URL}/openshift/templates/config/ia-search-config.yaml \
    | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml  \
   -f ${GIT_URL}/openshift/templates/config/ia-sftp-config.yaml \
    | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml  \
   -f ${GIT_URL}/openshift/templates/config/ia-throttle-config.yaml \
    | oc apply -f - -n ${TARGET_NAMESPACE}

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${TARGET_NAMESPACE}
