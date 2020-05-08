# Bootstrap
Setup openshift tools
## Jenkins
## Sonarqube
## RabbitMQ
## Jeager
# Deploy Applications
## Search-API
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE=${NAMESPACE_PREFIX}-

# Configuration evn/secrets
oc process -o=yaml \
  -f fams3/openshift/templates/config/rabbit-mq-configuration.yaml \
  -p HOST=  \
  -p PORT=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/searchapi-webhook-config.yaml \
  -p NAME=  \
  -p URL=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/redis-connection.yaml \
  -p HOST=  \
  -p PORT=  \
  | oc apply -f - -n ${NAMESPACE}

# Image stream
oc process -o=yaml \
  -f fams3/openshift/templates/builds/images/generic.yaml \
  -p namespacePrefix=  \
  -p appName=  \
  | oc apply -f - -n ${NAMESPACE}

# Build config
oc process -o=yaml \
  -f fams3/openshift/templates/builds/builds/search-api.yaml \
  -p namespacePrefix=  \
  | oc apply -f - -n ${NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f fams3/openshift/templates/builds/pipelines/search-api.yaml \
  -p namespacePrefix=  \
  | oc apply -f - -n ${NAMESPACE}
```
## Dynadapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE=${NAMESPACE_PREFIX}-

# Configuration evn/secrets
oc process -o=yaml \
  -f fams3/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/dynadapter-config.yaml \
  -p CRON=  \
  -p URL=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/dynamics-oauth-credentials.yaml \
  -p APIGATEWAY=  \
  -p CLIENTID=  \
  -p OAUTHURL=  \
  -p PASSWORD=  \
  -p RESOURCEURL=  \
  -p SECRET=  \
  -p USERNAME=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${NAMESPACE}

# Image stream
oc process -o=yaml \
  -f fams3/openshift/templates/builds/images/generic.yaml \
  -p namespacePrefix=  \
  -p appName=  \
  | oc apply -f - -n ${NAMESPACE}

# Build config
oc process -o=yaml \
  -f fams3/openshift/templates/builds/builds/dynadapter.yaml \
  -p namespacePrefix=  \
  | oc apply -f - -n ${NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f fams3/openshift/templates/builds/pipelines/dynadapter.yaml \
  -p namespacePrefix=  \
  | oc apply -f - -n ${NAMESPACE}
```
## Rest adapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE=${NAMESPACE_PREFIX}-
export DATAPARTNERSERVICE=

# Configuration evn/secrets
oc process -o=yaml \
  -f fams3/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/rest-config.yaml \
  -p APP_NAME=  \
  -p URL=  \
  -p CODE=  \
  -p USERNAME=  \
  -p PASSWORD=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${NAMESPACE}

# Image stream
oc process -o=yaml \
  -f fams3/openshift/templates/builds/images/rest-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}

# Build config
oc process -o=yaml \
  -f fams3/openshift/templates/builds/builds/rest-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f fams3/openshift/templates/builds/pipelines/rest-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}
```
## Web adapter
```shell script
export NAMESPACE_PREFIX=
export NAMESPACE=${NAMESPACE_PREFIX}-
export DATAPARTNERSERVICE=

# Configuration evn/secrets
oc process -o=yaml \
  -f fams3/openshift/templates/config/aspnet-env.yaml \
  -p ENVIRONMENT=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/web-adapter-config.yaml \
  -p APP_NAME=  \
  -p URL=  \
  -p PROFILE_NAME=  \
  | oc apply -f - -n ${NAMESPACE}

oc process -o=yaml \
  -f fams3/openshift/templates/config/jeager-config.yaml \
  -p URL=  \
  -p TYPE=  \
  | oc apply -f - -n ${NAMESPACE}

# Image stream
oc process -o=yaml \
  -f fams3/openshift/templates/builds/images/web-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}

# Build config
oc process -o=yaml \
  -f fams3/openshift/templates/builds/builds/web-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}

# Pipeline
oc process -o=yaml \
  -f fams3/openshift/templates/builds/pipelines/web-adapter.yaml \
  -p namespacePrefix=  \
  -p dataPartnerService=  \
  | oc apply -f - -n ${NAMESPACE}
```