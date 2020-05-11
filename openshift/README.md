# Bootstrap
Setup openshift tools
## Jenkins
### dotnet slave
```shell script
export NAMESPACE_PREFIX=
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/jenkins-slave-dotnet.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
### dotnet sonarqube slave
```shell script
export NAMESPACE_PREFIX=
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/jenkins-slave-sonarqube-dotnet.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
### zap slave
```shell script
export NAMESPACE_PREFIX=
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/jenkins-slave-zap.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
## Sonarqube
```shell script
export NAMESPACE_PREFIX=
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/sonarqube-postgresql-template.yaml \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
# Deploy Applications
## RabbitMQ
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# All-in-one template
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/rabbitmq.dc.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX} \
  | oc apply -f - ${TARGET_NAMESPACE}
```
## Jeager
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# All-in-one template
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/jaeger-aio.dc.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX} \
  | oc apply -f - ${TARGET_NAMESPACE}
```
## Redis
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

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
```
## Search-API
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

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

# Image stream
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  -p appName="search-api"  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}

# Build config
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/builds/search-api.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/pipelines/search-api.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
## Dynadapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

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

# Image stream
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/images/generic.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  -p appName="dynadapter"  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}

# Build config
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/builds/dynadapter.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f ${GIT_URL}/openshift/templates/builds/pipelines/dynadapter.yaml \
  -p namespacePrefix=${NAMESPACE_PREFIX}  \
  | oc apply -f - -n ${TOOLS_NAMESPACE}
```
## Rest adapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export DATAPARTNERSERVICE=
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

# Configuration evn/secrets
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
```
## Web adapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE_SUFFIX=
export TARGET_NAMESPACE=${NAMESPACE_PREFIX}-${NAMESPACE_SUFFIX}
export TOOLS_NAMESPACE=${NAMESPACE_PREFIX}-tools
export DATAPARTNERSERVICE=
export GIT_REPO="bcgov/fams3"
export GIT_BRANCH="master"
export GIT_URL="https://raw.githubusercontent.com/${GIT_REPO}/${GIT_BRANCH}"

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
```