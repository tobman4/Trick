FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY ./src/ /src/
RUN dotnet build -c Release -o /app /src

FROM mcr.microsoft.com/dotnet/aspnet:10.0

EXPOSE 8088

WORKDIR /app
COPY --from=build /app /app

CMD [ "dotnet", "Trick.dll" ]
