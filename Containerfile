FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build     
WORKDIR /app

# copy csproj and restore as distinct layers
COPY SecretsAPI.sln .
COPY nuget.config .
COPY SecretsAPI/*.csproj ./SecretsAPI/
RUN dotnet restore

# copy everything else and build app
COPY . ./
WORKDIR /app/SecretsAPI
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY dep-raspi/* ./

# Change timezone to local time
ENV TZ=America/Mexico_City
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Prepare everything for sqlite
RUN apt-get update && apt-get -y upgrade \
    && apt-get install -y --allow-unauthenticated \
    sqlite3 

COPY --from=build /app/SecretsAPI/out ./
ENTRYPOINT ["dotnet", "SecretsAPI.dll", "--urls", "http://0.0.0.0:5000;http://0.0.0.0:5001"]