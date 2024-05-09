# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM php:8.3-cli as base

ADD https://github.com/mlocati/docker-php-extension-installer/releases/latest/download/install-php-extensions /usr/local/bin/
RUN chmod +x /usr/local/bin/install-php-extensions \
  && install-php-extensions \
    opcache \
    pcntl \
    protobuf

# APM agent
RUN curl -sSO https://agent-binaries.cloud.solarwinds.com/apm/php/latest/solarwinds-apm-php.sh;sh solarwinds-apm-php.sh --service-key=<token>:apm-opentelemetry-demo-quoteservice

RUN echo "\nsolarwinds-apm.debug_level = 6" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.enable_cli = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\n" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini

WORKDIR /var/www
CMD php public/index.php
USER www-data
EXPOSE ${QUOTE_SERVICE_PORT}

FROM composer:2.7 AS vendor

WORKDIR /tmp/
COPY ./src/quoteservice/composer.json .

RUN composer install \
    --ignore-platform-reqs \
    --no-interaction \
    --no-plugins \
    --no-scripts \
    --no-dev \
    --prefer-dist

FROM base as final
COPY --from=vendor /tmp/vendor/ ./vendor/
COPY ./src/quoteservice/ /var/www

