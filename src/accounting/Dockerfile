# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0

FROM --platform=${BUILDPLATFORM} mcr.microsoft.com/dotnet/sdk:8.0 AS builder
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/src/accounting/", "Accounting/"]
COPY ["/pb/demo.proto", "Accounting/src/protos/demo.proto"]
RUN dotnet restore "./Accounting/Accounting.csproj" -r linux-$TARGETARCH
WORKDIR "/src/Accounting"

RUN dotnet build "./Accounting.csproj" -r linux-$TARGETARCH -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------------------------------------------------------

FROM builder AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Accounting.csproj" -r linux-$TARGETARCH -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/aspnet:8.0
USER app
WORKDIR /app
COPY --from=publish /app/publish .

USER root

# add solarwinds apm agent
RUN apt-get update && apt-get install -y curl

ENV SW_APM_HOME_DOTNET=/opt/solarwinds

RUN curl -sSO https://agent-binaries.cloud.solarwinds.com/apm/dotnet/latest/solarwinds-apm-dotnet-agent-package.linux.x86_64.tgz \
    && mkdir -p ${SW_APM_HOME_DOTNET} \
    && tar -zxvf solarwinds-apm-dotnet-agent-package.linux.x86_64.tgz -C ${SW_APM_HOME_DOTNET} \
    && rm solarwinds-apm-dotnet-agent-package.linux.x86_64.tgz

ENV CORECLR_PROFILER={AAAA7777-11FE-4F94-8EDA-312C0EDF4141}
ENV CORECLR_PROFILER_PATH=${SW_APM_HOME_DOTNET}/solarwinds_apm_profiler.so
ENV CORECLR_ENABLE_PROFILING=1

USER app
ENV OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES=Accounting.Consumer

ENTRYPOINT ["dotnet", "Accounting.dll"]
