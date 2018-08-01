FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64055
EXPOSE 44384

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY JobDeleteExample/JobDeleteExample.csproj JobDeleteExample/
RUN dotnet restore JobDeleteExample/JobDeleteExample.csproj
COPY . .
WORKDIR /src/JobDeleteExample
RUN dotnet build JobDeleteExample.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish JobDeleteExample.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JobDeleteExample.dll"]
