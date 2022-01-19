#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SIO.Mailer/SIO.Mailer.csproj", "src/SIO.Mailer/"]
COPY ["src/SIO.Migrations/SIO.Migrations.csproj", "src/SIO.Migrations/"]
COPY ["src/SIO.Domain/SIO.Domain.csproj", "src/SIO.Domain/"]
RUN dotnet restore "src/SIO.Mailer/SIO.Mailer.csproj"
COPY . .
WORKDIR "/src/src/SIO.Mailer"
RUN dotnet build "SIO.Mailer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SIO.Mailer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SIO.Mailer.dll"]