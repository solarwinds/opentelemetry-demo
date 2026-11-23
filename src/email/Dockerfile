# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM docker.io/library/ruby:4.0.10-alpine3.23@sha256:24be9067c09756e3c5d8315bc1c91d069e89b122b05d0f4f01eb06d7500308ea AS builder

COPY ./src/email/Gemfile Gemfile
COPY ./src/email/Gemfile.lock Gemfile.lock

RUN apk update && \
    apk add make gcc musl-dev gcompat && \
    apk add ruby-dev g++ make && \
    bundle install

FROM docker.io/library/ruby:4.0.10-alpine3.23@sha256:24be9067c09756e3c5d8315bc1c91d069e89b122b05d0f4f01eb06d7500308ea

RUN apk update && \
    apk add ruby-dev g++ make

COPY --from=builder /usr/local/bundle/ /usr/local/bundle/

WORKDIR /email_server

COPY ./src/email/views/ views/

COPY ./src/email/Gemfile Gemfile
COPY ./src/email/Gemfile.lock Gemfile.lock
COPY ./src/email/email_server.rb email_server.rb

ARG EMAIL_PORT=6060
EXPOSE ${EMAIL_PORT}
ENTRYPOINT ["bundle", "exec", "ruby", "email_server.rb"]
