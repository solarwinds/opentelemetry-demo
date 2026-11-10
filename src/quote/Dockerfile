# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM ghcr.io/mlocati/php-extension-installer:2.11.3@sha256:bfd14d8244f8fe22f40d5f0483c11d1b6dd5c31b801ef4238854ba78e65d5593 AS installer

FROM docker.io/library/composer:2.10.1@sha256:41959f55087549989efcdfe953977b64e98e07ca0d7532d7e4b7fe1a90cc4159 AS vendor

WORKDIR /tmp/

COPY ./src/quote/composer.json composer.json

RUN composer install \
    --ignore-platform-reqs \
    --no-interaction \
    --no-plugins \
    --no-scripts \
    --no-dev \
    --prefer-dist

FROM docker.io/library/php:8.5.12-cli-alpine3.23@sha256:21b15e527ce55b41a2beb0b4d31868a9a3fd37b8c5f094e4c8f5b7bbac017129

COPY --from=installer /usr/bin/install-php-extensions /usr/local/bin/

RUN install-php-extensions opcache pcntl

# PEAR's own downloader gets truncated behind TLS-inspecting proxies; curl detects that and retries.
RUN set -eux; \
    apk add --no-cache --virtual .build-deps $PHPIZE_DEPS; \
    for m in protobuf opentelemetry; do \
        v=$(curl -fsS --retry 10 --retry-all-errors https://pecl.php.net/rest/r/$m/stable.txt); \
        curl -fsS --retry 10 --retry-all-errors -o /tmp/$m.tgz https://pecl.php.net/get/$m-$v.tgz; \
        printf '\n' | pecl install /tmp/$m.tgz; \
        docker-php-ext-enable $m; \
    done; \
    rm -f /tmp/*.tgz; \
    apk del .build-deps

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

EXPOSE ${QUOTE_PORT}

CMD ["php", "public/index.php"]