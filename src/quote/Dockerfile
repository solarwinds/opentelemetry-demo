# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM ghcr.io/mlocati/php-extension-installer:2.11.12@sha256:b6d3fa381b9ba5cf051117c1c601d6a523b590e534bf3d56eb4fbe352949c138 AS installer

FROM docker.io/library/composer:2.10.3@sha256:d8f6343d3fae98107426bc49163ccad46ef85aabd4a27d80a74401fab4aba332 AS vendor

WORKDIR /tmp/

COPY ./src/quote/composer.json composer.json

RUN composer install \
    --ignore-platform-reqs \
    --no-interaction \
    --no-plugins \
    --no-scripts \
    --no-dev \
    --prefer-dist

FROM docker.io/library/php:8.5.12-cli-alpine3.24@sha256:1ff5b45922ab30f418447c6089dbe2eba5f01e0b2742863d0327ac349305d70f

COPY --from=installer /usr/bin/install-php-extensions /usr/local/bin/

RUN install-php-extensions opcache pcntl protobuf opentelemetry

# Install bash, dos2unix, and libstdc++ for SolarWinds APM installation
RUN apk add --no-cache bash dos2unix libstdc++

# Install the SolarWinds APM PHP agent
RUN curl -sS -o /usr/local/bin/solarwinds-apm-php.sh \
    https://ssp-prod-global-agent-binaries.s3.amazonaws.com/apm/php/latest/solarwinds-apm-php.sh \
 && dos2unix /usr/local/bin/solarwinds-apm-php.sh \
 && chmod +x /usr/local/bin/solarwinds-apm-php.sh

# service-key is mandatory parameter, specific service key will be provided by env variable
RUN bash /usr/local/bin/solarwinds-apm-php.sh \
    --service-key="00000000-0000-0000-0000-000000000000:astronomyshop-quote"

RUN echo "solarwinds-apm.enable_cli = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "solarwinds-apm.use_custom_start_trace = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "solarwinds-apm.enable_profiling = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
# End of SolarWinds APM PHP agent installation

WORKDIR /var/www

USER www-data

COPY --from=vendor /tmp/vendor/ vendor/

COPY ./src/quote/app/ app/
COPY ./src/quote/public/ public/
COPY ./src/quote/src/ src/

ARG QUOTE_PORT=8090
EXPOSE ${QUOTE_PORT}

CMD ["php", "public/index.php"]