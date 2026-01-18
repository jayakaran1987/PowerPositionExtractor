# PowerPositionExtractor
Development Challenge
## Overview
This application retrieves power trades from a given service, aggregates the data and generates a report to a configured storage location at a scheduled interval.

## Adjustments from Original Challenge

- **Background Service vs Windows Service**: Built as a .NET Background Service instead of a Windows Service to support Kubernetes deployment and containerization
- **Flexible Storage Strategy**: considered storagePath supports both local file systems and cloud storage providers
- **Why is Kubernetes CronJob not used? Because it is not acceptable to miss a scheduled extract.

## Configuration

Configuration is managed for locally `appsettings.json`,  for Kubernetes deployment under the  helm values file :

```json
{
  "ExtractorOptions": {
    "StoragePath": "/somepath/reports",
    "LocalTimeZone": "Europe/London",
    "ScheduledTimeIntervalInMinutes": 60,
    "StorageStrategy": "Local"
  }
}
```
## Docker (tested locally)

```bash
# Build
docker build -f docker/Dockerfile -t power-position-worker:0.1 .
```

## Kubernetes Deployment (not tested)
created helm charts and value files 
