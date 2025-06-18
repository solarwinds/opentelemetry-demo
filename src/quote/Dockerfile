# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM php:8.3-cli AS base

RUN apt-get update && apt-get install -y bash dos2unix
ADD https://github.com/mlocati/docker-php-extension-installer/releases/latest/download/install-php-extensions /usr/local/bin/
RUN chmod +x /usr/local/bin/install-php-extensions \
  && install-php-extensions \
    opcache \
    pcntl \
    protobuf \
    opentelemetry

RUN curl -sS -o /usr/local/bin/solarwinds-apm-php.sh \
    https://ssp-prod-global-agent-binaries.s3.amazonaws.com/apm/php/latest/solarwinds-apm-php.sh \
 && dos2unix /usr/local/bin/solarwinds-apm-php.sh \
 && chmod +x /usr/local/bin/solarwinds-apm-php.sh

# service-key is mandatory parameter, specific service key will be provided by env variable
RUN bash /usr/local/bin/solarwinds-apm-php.sh \
    --service-key="00000000-0000-0000-0000-000000000000:astronomyshop-quote"

RUN echo "\nsolarwinds-apm.debug_level = 6" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.enable_cli = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.use_custom_start_trace = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\nsolarwinds-apm.enable_profiling = 1" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini
RUN echo "\n" >> /usr/local/etc/php/conf.d/solarwinds-apm.ini

WORKDIR /var/www
CMD ["php", "public/index.php"]
USER www-data
EXPOSE ${QUOTE_PORT}

FROM composer:2.7 AS vendor

WORKDIR /tmp/
COPY ./src/quote/composer.json .

RUN composer install \
    --ignore-platform-reqs \
    --no-interaction \
    --no-plugins \
    --no-scripts \
    --no-dev \
    --prefer-dist

FROM base AS final
COPY --from=vendor /tmp/vendor/ ./vendor/
COPY ./src/quote/ /var/www
