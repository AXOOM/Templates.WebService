# My Service Helm Chart

This Helm chart deploys My Service.

my description

Use [Quberneeds](https://github.com/AXOOM/Quberneeds) to deploy this chart with the following environment variables:

| Name                     | Default       | Description                                                                                |
|--------------------------|---------------|--------------------------------------------------------------------------------------------|
| `TENANT_ID`              | (required)    | The ID of the customer instance (used as the Kubernetes namespace).                        |
| `PUBLIC_DOMAIN`          | (required)    | The public DNS prefix the entire namespace is exposed under.                               |
| `ASPNETCORE_ENVIRONMENT` | `Production`  | Set to `Development` to enable debug logging, Swagger documentation, exception pages, etc. |
