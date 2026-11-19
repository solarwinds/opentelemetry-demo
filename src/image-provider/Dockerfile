# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0

FROM docker.io/library/nginx:1.30.7-alpine3.24-otel@sha256:a05474d618c6a5da1081380cb54728f53743bc295b04f6dc5378a6282c90d190

RUN chown -R 101:0 /etc/nginx /var/cache/nginx \
    && chmod -R g+w /etc/nginx /var/cache/nginx

USER 101

COPY src/image-provider/static/ /static/
COPY src/image-provider/nginx.conf.template /nginx.conf.template

ARG IMAGE_PROVIDER_PORT=8081
EXPOSE ${IMAGE_PROVIDER_PORT}

STOPSIGNAL SIGQUIT

# Start nginx
CMD ["/bin/sh" , "-c" , "envsubst '$OTEL_COLLECTOR_HOST $IMAGE_PROVIDER_PORT $OTEL_COLLECTOR_PORT_GRPC $OTEL_SERVICE_NAME' < /nginx.conf.template > /etc/nginx/nginx.conf && cat  /etc/nginx/nginx.conf && exec nginx -g 'daemon off;'"]
