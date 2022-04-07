![master](https://github.com/JerryBian/laobian.me/workflows/master/badge.svg)

Source code of building my peronal blog website: https://blog.laobian.me


### Docker image

The prebuilt docker image can be found either on [Docker Hub](https://hub.docker.com/r/cnbian/laobian-blog) or [GitHub Package](https://github.com/JerryBian/laobian.me/pkgs/container/laobian-blog). To quickly startup the applications, pass variables to [docker-compose.yml](./docker-compose.yml).

### Build from source

#### Required tools:
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/)
- [.NET 6](https://dotnet.microsoft.com/)
- [Powershell](https://github.com/PowerShell/PowerShell)
- [Node.js](https://nodejs.org/en/)

#### Initialize

1. Run `npm install`
2. Run `script/setup-dev-secrets.ps1`
3. Run `npm run build`

### License

Powered by .NET technology, licensed under [MIT](./LICENSE).