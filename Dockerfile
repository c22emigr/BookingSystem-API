# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/BookingApi/BookingApi.csproj", "src/BookingApi/"]
RUN dotnet restore "src/BookingApi/BookingApi.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/src/BookingApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookingApi.dll"]