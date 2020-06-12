#!/bin/bash

# Wait for splunk to startup
while [ "${HEALTH_CHECK}" != "{\"text\":\"HEC is healthy\",\"code\":17}" ]
do
  echo "Waiting for splunk ..."
  HEALTH_CHECK=$(curl -ksS https://splunk:8088/services/collector/health)
  sleep 3
done

sleep 10

# Create app
echo "Creating app: fams3 ..."
curl -k --user admin:"${SPLUNK_PASSWORD}" https://splunk:8089/services/apps/local \
     -d name=fams3

# Create dashboards
echo "Creating dashboards ..."
DASHBOARD=$(cat dashboards/fams3_executive_summary.xml | sed -e "s/&gt;/>/g" -e "s/\([[:alnum:]]*\) &lt; \([[:alnum:]]*\)/\2 > \1/g" -e '/<html>/{:a;N;/<\/html>/!ba};/font/d')
#DASHBOARD="<dashboard><label>FAMS3 Executive Summary</label></dashboard>"
echo ${DASHBOARD}
curl -k --user admin:"${SPLUNK_PASSWORD}" https://splunk:8089/servicesNS/admin/fams3/data/ui/views \
     -d "name=fams3_executive_summary&eai:data=${DASHBOARD}"

DASHBOARD=$(cat dashboards/fams3_ops_dashboard.xml)
#DASHBOARD="<dashboard><label>FAMS3 Ops</label></dashboard>"
echo ${DASHBOARD}
curl -k --user admin:"${SPLUNK_PASSWORD}" https://splunk:8089/servicesNS/admin/fams3/data/ui/views \
     -d "name=fams3_ops_dashboard&eai:data=${DASHBOARD}"

# Create index
echo "Creating index ..."
curl -k --user admin:"${SPLUNK_PASSWORD}" https://splunk:8089/servicesNS/admin/fams3/data/indexes  \
     -d name=dev_fams  \
     -d datatype=event

# Create HEC token
sleep 5
echo "Creating HEC token ..."
curl -ksS --user admin:"${SPLUNK_PASSWORD}" https://splunk:8089/servicesNS/admin/fams3/data/inputs/http \
     -d name=fams_hec_token \
     -d index=dev_fams \
     -d token="${SPLUNK_TOKEN}"

# Test
echo "Testing log write ..."
curl -ksS "https://splunk:8088/services/collector" \
     -H "Authorization: Splunk ${SPLUNK_TOKEN}"  \
     -d '{"event": "Hello, world!", "sourcetype": "manual"}'
