FROM alpine:3.19.1

RUN apk add --no-cache curl bash sed

COPY . /app
WORKDIR /app

RUN sed -i 's/\r//g' init.sh

ENTRYPOINT ["/bin/bash", "init.sh"]
