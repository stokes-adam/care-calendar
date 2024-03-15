FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./api/*.csproj ./api/
COPY ./model/*.csproj ./model/
COPY ./service/*.csproj ./service/
RUN dotnet restore "./api/api.csproj"

COPY . .
WORKDIR /app/api
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/api/out .

ENTRYPOINT ["dotnet", "api.dll"]
