# Deployment of Astronomy shop application instrumented for SWO to k8s cluster

## Deploy k8s manifests

```
kubectl apply -f opentelemetry-demo.yaml -n otel-demo
```

## Adjust configuration

SWO configuration variables reside in `swo-config` ConfigMap. For more information about endpoints go to [SolarWinds Observability | Data centers and endpoint URIs](https://documentation.solarwinds.com/en/success_center/observability/content/system_requirements/endpoints.htm).

```
apiVersion: v1
kind: ConfigMap
metadata:
  name: swo-config
data:
  OTEL_ADDRESS: otel.collector.na-01.cloud.solarwinds.com:443 # OTLP telemetry data ingestion endpoint
  PUBLIC_OTEL_EXPORTER_OTLP_TRACES_ENDPOINT: "" # used to send traces from website
  SW_APM_COLLECTOR: apm.collector.na-01.cloud.solarwinds.com # APM collector
  SWO_RUM_SCRIPT: "" # URL to script for DEM RUM
  SWO_URL: na-01.cloud.solarwinds.com # Endpoint to send data from UAMS Agent
  UI_EXTERNAL_URL: "http://frontend-proxy:8080" # enter external url for astronomy shop website, e.g. load balancer address
  UPSTREAM_DISTRO_DISABLED: "true" # to monitor only services which have native SWO agent
```

## Provide SWO ingestion token

```
kubectl create secret generic swo-apm --from-literal=token=<swo_ingestion_token> -n otel-demo
```
