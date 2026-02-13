# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM --platform=${BUILDPLATFORM} gradle:8-jdk17 AS builder

WORKDIR /usr/src/app/

COPY ./src/fraud-detection/ ./
COPY ./pb/ ./src/main/proto/

RUN gradle shadowJar

# -----------------------------------------------------------------------------

FROM gcr.io/distroless/java17-debian12:nonroot

ARG OTEL_JAVA_AGENT_VERSION

WORKDIR /usr/src/app/

ADD --chmod=644 https://agent-binaries.cloud.solarwinds.com/apm/java/latest/solarwinds-apm-agent.jar /usr/src/app/solarwinds-apm-agent.jar

ENV JAVA_TOOL_OPTIONS=-javaagent:/usr/src/app/solarwinds-apm-agent.jar

COPY --from=builder /usr/src/app/build/libs/fraud-detection-1.0-all.jar fraud-detection-1.0-all.jar

ENTRYPOINT [ "java", "-jar", "fraud-detection-1.0-all.jar" ]
