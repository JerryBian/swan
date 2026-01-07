FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ARG ver=1.0

COPY ./src ./

RUN dotnet publish \
    -c Release \
    -o /publish \
    -v normal \
    /property:Version=${ver} \
    ./web/Swan.csproj

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /publish ./

ENV TZ=Asia/Shanghai

CMD ["dotnet", "swan.dll"]