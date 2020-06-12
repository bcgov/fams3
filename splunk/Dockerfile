#############################################################################################
###              Stage where Docker is building curl container to setup Splunk            ###
#############################################################################################
FROM alpine:3.12.0 as build

RUN apk add --no-cache curl bash sed

COPY . .

# Remove non-unix newline character
RUN sed -i 's/\r//g' init.sh

ENTRYPOINT ["/bin/bash", "init.sh"]
#############################################################################################
