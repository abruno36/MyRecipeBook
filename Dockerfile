# =============================
#   STAGE 1 — Build
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src ./src

WORKDIR /src/src/Backend/MyRecipeBook.API

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# =============================
#   STAGE 2 — Runtime
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true

# instala netcat corretamente
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

COPY wait-for-sql.sh /wait-for-sql.sh
RUN chmod +x /wait-for-sql.sh

EXPOSE 8080

ENTRYPOINT ["/wait-for-sql.sh", "dotnet", "MyRecipeBook.API.dll"]