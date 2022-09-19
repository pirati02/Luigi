FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Domain/Reservation/Reservation.Api/Reservation.Api.csproj", "Reservation.Api/"]
RUN dotnet restore "src/Domain/Reservation/Reservation.Api/Reservation.Api.csproj"
COPY . .
WORKDIR "/src/Reservation.Api"
RUN dotnet build "Reservation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reservation.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reservation.Api.dll"]
