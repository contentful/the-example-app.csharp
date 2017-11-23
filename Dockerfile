FROM microsoft/aspnetcore-build:2.0 AS builder
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/

ENV ASPNETCORE_ENVIRONMENT=Heroku

FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "TheExampleApp.dll"]