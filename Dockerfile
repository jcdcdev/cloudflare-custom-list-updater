# --- Build stage ---
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG VERSION=0.0.0-dev
WORKDIR /src

# Copy project file and restore first (layer caching).
COPY jcdcdev.Cloudflare.CustomListSync/jcdcdev.Cloudflare.CustomListSync.csproj ./jcdcdev.Cloudflare.CustomListSync/
RUN dotnet restore jcdcdev.Cloudflare.CustomListSync/jcdcdev.Cloudflare.CustomListSync.csproj

# Copy source and publish.
COPY jcdcdev.Cloudflare.CustomListSync/ ./jcdcdev.Cloudflare.CustomListSync/
RUN dotnet publish jcdcdev.Cloudflare.CustomListSync/jcdcdev.Cloudflare.CustomListSync.csproj \
    -c Release \
    -o /app/publish \
    -p:Version=${VERSION}

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

ARG VERSION=0.0.0-dev
LABEL org.opencontainers.image.version=${VERSION}

# Create non-root user.
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

WORKDIR /app
COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

ENTRYPOINT ["dotnet", "jcdcdev.Cloudflare.CustomListSync.dll"]
