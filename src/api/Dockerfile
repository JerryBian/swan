FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./ ./
RUN dotnet publish -c Release -o /publish --self-contained true -v normal -p:PublishTrimmed=true -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /publish ./
RUN chmod +x laobian.api

ENTRYPOINT ["./laobian.api"]