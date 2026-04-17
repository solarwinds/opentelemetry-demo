<!-- markdownlint-disable-next-line -->
# <img src="https://opentelemetry.io/img/logos/opentelemetry-logo-nav.png" alt="OTel logo" width="45"> OpenTelemetry Demo with SolarWinds Observability

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg?color=red)](https://github.com/open-telemetry/opentelemetry-demo/blob/main/LICENSE)

## About

This repository is a fork of the [OpenTelemetry Astronomy Shop Demo](https://github.com/open-telemetry/opentelemetry-demo),
a microservice-based distributed system that illustrates the implementation of
OpenTelemetry in a near real-world environment.

This fork demonstrates how the Astronomy Shop can be monitored using
[SolarWinds Observability](https://www.solarwinds.com/solutions/solarwinds-observability/apm).
Service implementations, Dockerfiles, and Kubernetes manifests have been adjusted
to send telemetry data to SolarWinds Observability for full-stack application
performance monitoring and troubleshooting.

You can explore a live deployment of this demo in the
[SolarWinds Observability Online Demo](https://demo.na-01.cloud.solarwinds.com/).

## Prerequisites

Before deploying the demo, you need a
[SolarWinds Observability SaaS](https://www.solarwinds.com/solarwinds-observability)
account and the following configuration values:

1. **Ingestion API token** — create one in SolarWinds Observability SaaS under
   *Settings > API Tokens* (select the **Ingestion** token type).
   See [API Tokens](https://documentation.solarwinds.com/en/success_center/observability/content/settings/api-tokens.htm)
   for details.

2. **Data center endpoints** — endpoint URIs depend on the data center your
   organization uses (visible in your SWO URL, e.g. `na-01`, `na-02`, `eu-01`,
   `ap-01`). Replace `xx-yy` in the URIs below with your data center identifier.
   See [Data centers and endpoint URIs](https://documentation.solarwinds.com/en/success_center/observability/content/system_requirements/endpoints.htm)
   for the full list.

   | Variable | Value | Purpose |
   |---|---|---|
   | `SW_APM_SERVICE_TOKEN` | Your ingestion API token | Authenticates telemetry sent to SWO |
   | `SW_APM_COLLECTOR` | `apm.collector.xx-yy.cloud.solarwinds.com` | APM collector endpoint |
   | `SW_OTEL_ADDRESS` | `otel.collector.xx-yy.cloud.solarwinds.com` | OTLP telemetry data ingestion endpoint (port 443) |

For information on how to add individual services to monitoring, see
[Add a service](https://documentation.solarwinds.com/en/success_center/observability/content/configure/configure-services.htm).

## Deploy with Docker

1. Clone the repository:

   ```shell
   git clone https://github.com/solarwinds/opentelemetry-demo.git
   cd opentelemetry-demo
   ```

2. Set the SolarWinds environment variables in the `.env` file:

   ```dotenv
   # SolarWinds Observability
   SW_APM_SERVICE_TOKEN=<your_ingestion_token>
   SW_APM_COLLECTOR=apm.collector.xx-yy.cloud.solarwinds.com
   SW_OTEL_ADDRESS=otel.collector.xx-yy.cloud.solarwinds.com
   ```

   > **Important:** Replace `<your_ingestion_token>` with the token created in
   > the [Prerequisites](#prerequisites) and `xx-yy` with your data center
   > identifier.

3. Start the demo:

   ```shell
   docker compose up -d
   ```

4. Once running, the web store is available at <http://localhost:8080/>.

## Deploy with Kubernetes

1. Create the target namespace:

   ```shell
   kubectl create namespace otel-demo
   ```

2. Create the secret with your SWO ingestion token:

   ```shell
   kubectl create secret generic swo-apm \
     --from-literal=token=<your_ingestion_token> \
     -n otel-demo
   ```

3. Review and update the `swo-config` ConfigMap inside
   `kubernetes/opentelemetry-demo.yaml` with the endpoints matching your data
   center (replace `na-01` if needed):

   ```yaml
   apiVersion: v1
   kind: ConfigMap
   metadata:
     name: swo-config
   data:
     OTEL_ADDRESS: otel.collector.na-01.cloud.solarwinds.com:443
     SW_APM_COLLECTOR: apm.collector.na-01.cloud.solarwinds.com
   ```

4. Apply the manifests:

   ```shell
   kubectl apply -f kubernetes/opentelemetry-demo.yaml -n otel-demo
   ```

See [kubernetes/README.md](kubernetes/README.md) for additional Kubernetes
configuration options.

> **Note:** For configuring Kubernetes monitoring in SolarWinds Observability,
> see [Kubernetes monitoring](https://documentation.solarwinds.com/en/success_center/observability/content/intro/kubernetes.htm).

## Documentation

For general documentation on the upstream demo, see the
[OpenTelemetry Demo Documentation](https://opentelemetry.io/docs/demo/).

For SolarWinds Observability, see the
[SolarWinds documentation](https://documentation.solarwinds.com/).

## Upstream

This fork is based on the
[open-telemetry/opentelemetry-demo](https://github.com/open-telemetry/opentelemetry-demo)
project. See the upstream repository for the full list of
[contributors](https://github.com/open-telemetry/opentelemetry-demo/graphs/contributors).

## License

This project is licensed under the Apache 2.0 License — see the [LICENSE](LICENSE) file for details.
