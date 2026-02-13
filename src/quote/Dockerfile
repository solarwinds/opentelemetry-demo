# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM ghcr.io/mlocati/php-extension-installer:2.9.11 AS installer

FROM docker.io/library/composer:2.8.12 AS vendor

WORKDIR /tmp/

COPY ./src/quote/composer.json composer.json

RUN composer install \
    --ignore-platform-reqs \
    --no-interaction \
    --no-plugins \
    --no-scripts \
    --no-dev \
    --prefer-dist

FROM docker.io/library/php:8.4-cli-alpine3.22

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

RUN echo "\nsolarwinds-apm.enable_cli = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.use_custom_start_trace = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.enable_profiling = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\n" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
# End of SolarWinds APM PHP agent installation

WORKDIR /var/www

USER www-data

COPY --from=vendor /tmp/vendor/ vendor/

COPY ./src/quote/app/ app/
COPY ./src/quote/public/ public/
COPY ./src/quote/src/ src/

EXPOSE ${QUOTE_PORT}

CMD ["php", "public/index.php"]