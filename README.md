# cloudflare-custom-list-updater

[![Docker Image Version](https://img.shields.io/docker/v/jcdcdev/cloudflare-custom-list-updater/latest)](https://hub.docker.com/r/jcdcdev/cloudflare-custom-list-updater)
[![Docker Image Size](https://img.shields.io/docker/image-size/jcdcdev/cloudflare-custom-list-updater/latest)](https://hub.docker.com/r/jcdcdev/cloudflare-custom-list-updater)
[![GitHub issues](https://img.shields.io/github/issues/jcdcdev/cloudflare-custom-list-updater)](https://github.com/jcdcdev/cloudflare-custom-list-updater/issues)
[![GitHub last commit](https://img.shields.io/github/last-commit/jcdcdev/cloudflare-custom-list-updater)](https://github.com/jcdcdev/cloudflare-custom-list-updater/commits)

A .NET 10 Docker container that declaratively manages a Cloudflare custom list.

- Auto-detect the device's public IP (IPv4, IPv6, or both)
- Hardcode entries via environment variables or load from a CSV file
- Periodic sync every 5 minutes (configurable)

## Quick Start

### Docker Compose

```yaml title="docker-compose.yml"
services:
  cloudflare-custom-list-updater:
    image: jcdcdev/cloudflare-custom-list-updater:alpha
    build: .
    environment:
      CF_API_TOKEN: "your-cloudflare-api-token"
      CF_ACCOUNT_ID: "your-account-id"
      CF_LIST_IDS: "your-list-id"
      CF_ENTRIES: "my-device=ipv4;my-office=1.2.3.4"
    restart: unless-stopped
```

```bash
docker compose up -d
```

The container will detect your public IP and keep the Cloudflare list entry updated every 5 minutes.

## Configuration

All configuration is via environment variables with the `CF_` prefix:

| Variable                   | Default      | Description                                                    |
|----------------------------|--------------|----------------------------------------------------------------|
| `CF_API_TOKEN`             | *(required)* | Cloudflare API token with Account Filter Lists Edit permission |
| `CF_ACCOUNT_ID`            | *(required)* | Cloudflare account ID                                          |
| `CF_LIST_IDS`              | *(required)* | Comma-separated list IDs to sync                               |
| `CF_SYNC_INTERVAL_SECONDS` | `300`        | Seconds between sync cycles (5 min default)                    |
| `CF_KEEP_ORPHANS`          | `false`      | When true, entries not in config are preserved                 |
| `CF_FILE_PATH`             | *(optional)* | Path to CSV file for entry configuration (CSV mode)            |
| `CF_ENTRIES`               | *(optional)* | Semicolon-separated KVP entries (KVP mode)                     |

### Input Modes

The tool supports two configuration modes, selected by which variables are set (priority order: CSV > ENV):

#### Env Mode (single or multiple entries)

Set `CF_ENTRIES` with semicolon-separated `Label=Address` pairs:

```yaml
environment:
  CF_ENTRIES: "my-device=ipv4"  # single entry with auto-detect
```

Or for multiple entries:

```yaml
environment:
  CF_ENTRIES: "home=ipv4;dns-server=ipv6;office=10.0.0.1;vpn=172.16.0.1"
```

The value determines how the entry is resolved:

| Address         | Behavior                   |
|-----------------|----------------------------|
| `ipv4`          | Auto-detect public IPv4    |
| `ipv6`          | Auto-detect public IPv6    |
| Any other value | Use as-is (static IP/CIDR) |

#### CSV Mode (file-based)

Set `CF_FILE_PATH` to a mounted CSV file:

```yaml
volumes:
  - ./entries.csv:/app/entries.csv:ro
environment:
  CF_FILE_PATH: "/app/entries.csv"
```

CSV format (header row required):

```csv
Label,Address
home-server,ipv4
dns-server,ipv6
office-ip,10.0.0.1
vpn-gateway,172.16.0.1
```

## Cloudflare API Token Setup

1. Go to [Cloudflare Dashboard](https://dash.cloudflare.com/profile/api-tokens)
2. Click **Create Token**
3. Use **Custom Token** with these permissions:
    - **Account** → **Filter Lists** → **Edit**
4. Copy the token value to `CF_API_TOKEN`

## Development

### Local Configuration

For local development and debugging, use launch profiles with the `--launch-profile` flag:

```bash
# Run with dev profile
dotnet run --project src/jcdcdev.Cloudflare.CustomListSync --launch-profile dev
```

Create or modify `Properties/launchSettings.json` to set environment variables for a profile:

```json
{
  "profiles": {
    "dev": {
      "environmentVariables": {
        "CF_API_TOKEN": "your-cloudflare-api-token",
        "CF_ACCOUNT_ID": "your-account-id",
        "CF_LIST_IDS": "your-list-id",
        "CF_FILE_PATH": "/path/to/entries.csv",
        "CF_ENTRIES": "home=ipv4;office=10.0.0.1"
      }
    }
  }
}
```

### Running Locally

```bash
# Build
dotnet build jcdcdev.Cloudflare.CustomListSync.sln

# Run tests
dotnet test jcdcdev.Cloudflare.CustomListSync.sln --verbosity normal

# Run locally
dotnet run --project src/jcdcdev.Cloudflare.CustomListSync --launch-profile dev
```

## Architecture

- **.NET 10** console app running as a `BackgroundService`
- **Structured logging** via Microsoft.Extensions.Logging
- **Cloudflare Rules Lists API** via typed HttpClient with Polly retry
- **Multiple config providers** (ENV, CSV) selected by priority
- **Multiservice IP detection** with automatic fallback
- **Alpine Docker image** with self-contained publish, non-root user
