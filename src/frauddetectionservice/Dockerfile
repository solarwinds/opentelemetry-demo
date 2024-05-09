# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


FROM gradle:8-jdk17 AS builder

WORKDIR /usr/src/app/

COPY ./src/frauddetectionservice/ ./
COPY ./pb/ ./src/main/proto/
RUN gradle shadowJar

# -----------------------------------------------------------------------------

FROM gcr.io/distroless/java17-debian11

ARG version=2.3.0
WORKDIR /usr/src/app/

COPY --from=builder /usr/src/app/build/libs/frauddetectionservice-1.0-all.jar ./
ADD --chmod=644 https://agent-binaries.cloud.solarwinds.com/apm/java/$version/solarwinds-apm-agent.jar /app/opentelemetry-javaagent.jar
ENV SW_APM_COLLECTOR=apm.collector.na-01.st-ssp.solarwinds.com
ENV SW_APM_SERVICE_KEY=<service-key>
ENV JAVA_TOOL_OPTIONS=-javaagent:/app/opentelemetry-javaagent.jar

ENTRYPOINT [ "java", "-jar", "frauddetectionservice-1.0-all.jar" ]
