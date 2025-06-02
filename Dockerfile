# Base runtime image for final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore
COPY ["InternPulse4.csproj", "./"]
RUN dotnet restore "InternPulse4.csproj"

# Copy the rest of the source
COPY . .
RUN dotnet build "InternPulse4.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish app
FROM build AS publish
RUN dotnet publish "InternPulse4.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InternPulse4.dll"]
