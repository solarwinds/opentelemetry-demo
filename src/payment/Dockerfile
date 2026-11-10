# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM docker.io/library/node:26.13.0-slim@sha256:5adad64f9a1fca2d51972caa65983634c8a774eb300d72b20f7f2a0f60368072 AS builder

WORKDIR /usr/src/app/

COPY ./src/payment/package.json package.json
COPY ./src/payment/package-lock.json package-lock.json

RUN npm ci --omit=dev

# -----------------------------------------------------------------------------

FROM gcr.io/distroless/nodejs22-debian13:nonroot@sha256:1b90ca047c3a1046ad3527998c747c25b6a88dde91ba8c71b98669d5813aaa21

WORKDIR /usr/src/app/

COPY --from=builder /usr/src/app/node_modules/ node_modules/

COPY ./pb/demo.proto demo.proto

COPY ./src/payment/charge.js charge.js
COPY ./src/payment/index.js index.js
COPY ./src/payment/logger.js logger.js
COPY ./src/payment/opentelemetry.js opentelemetry.js

EXPOSE ${PAYMENT_PORT}

ENTRYPOINT ["/nodejs/bin/node", "--import", "solarwinds-apm", "index.js"]
