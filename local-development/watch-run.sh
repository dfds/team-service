#!/bin/bash
CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING="User ID=postgres;Password=p;Host=localhost;Port=5432;Database=capabilitydb;" \
CAPABILITY_SERVICE_KAFKA_BOOTSTRAP_SERVERS=localhost:9092 \
dotnet watch --project ./../src/CapabilityService.WebApi/CapabilityService.WebApi.csproj \
run --server.urls "http://*:50900" --no-launch-profile