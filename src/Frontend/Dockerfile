FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Frontend/Frontend.csproj Frontend/
COPY Entities/Entities.csproj Entities/
RUN dotnet restore Frontend/Frontend.csproj
COPY . .
WORKDIR /src/Frontend
RUN dotnet build Frontend.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Frontend.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Frontend.dll"]
