FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build

ENV ASPNETCORE_URLS=http://+:5057

ENTRYPOINT ["dotnet", "watch", "run", "--no-launch-profile"]
