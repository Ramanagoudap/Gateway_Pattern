# Gateway Pattern (YARP)

Demo API gateway with two downstream microservices.

## Solution layout

```
src/
  ApiGateway/              YARP reverse proxy + gateway policies
  Orders.Service/          Orders API (also calls Notifications on create)
  Notifications.Service/   Notifications API
```

## Ports

| Service        | URL                      |
|----------------|--------------------------|
| ApiGateway     | http://localhost:5172    |
| Orders         | http://localhost:5180    |
| Notifications  | http://localhost:5181    |

Redis (optional): `localhost:6379` — cache/idempotency bypass gracefully if unavailable.

## Request flow

1. Client calls **ApiGateway** only (never the services directly in the happy path).
2. Middleware: correlation ID → idempotency → GET response cache.
3. **YARP** matches the path and proxies:
   - `/api/v1/orders/**` → Orders (`5180`)
   - `/api/v1/notifications/**` → Notifications (`5181`)
4. On order create, **Orders → Notifications** is a direct service-to-service call (not through the gateway).

```text
Client
  -> ApiGateway :5172
       -> YARP
            -> Orders :5180
                 -> Notifications :5181   (side effect on POST order)
            -> Notifications :5181        (when client calls /api/v1/notifications)
```

## Run all services

### Cursor / VS Code

Use the compound launch config **All Services** (`.vscode/launch.json`).

### CLI

```powershell
dotnet run --project src/Notifications.Service --launch-profile http
dotnet run --project src/Orders.Service --launch-profile http
dotnet run --project src/ApiGateway --launch-profile http
```

### Visual Studio

Open `Gateway_Pattern.slnx`, set multiple startup projects (all three), then Start.

## Smoke test

Use [`src/ApiGateway/ApiGateway.http`](src/ApiGateway/ApiGateway.http):

1. `POST /api/v1/orders` through the gateway
2. `GET /api/v1/notifications` — should include the order side-effect notification

## Config notes

- YARP routes/clusters: `src/ApiGateway/appsettings.json` → `ReverseProxy`
- Cache/idempotency TTLs and path prefixes: `Gateway` section (`ProxiedPathPrefixes` should stay aligned with YARP route prefixes)
