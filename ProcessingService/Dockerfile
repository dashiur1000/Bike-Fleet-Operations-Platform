FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProcessingService.csproj", "./"]
RUN dotnet restore "ProcessingService.csproj"

COPY . .
RUN dotnet publish "ProcessingService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProcessingService.dll"]