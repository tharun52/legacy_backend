# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Restore dependencies first (maximises layer cache reuse)
COPY BlogApi.csproj .
RUN dotnet restore BlogApi.csproj

# Copy remaining source and publish a self-contained release
COPY . .
RUN dotnet publish BlogApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Non-root user for security hardening
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

COPY --from=build /app/publish .

USER appuser

EXPOSE 8080

# Inject at runtime via environment variable or Kubernetes secret — never hardcode
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD wget -qO- http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BlogApi.dll"]
