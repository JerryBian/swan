Source of building my personal websites.

There are three branches for different purposes:

- master: This is the source for production environment, running at https://blog.laobian.me
- stage: This is the source for staging environment. While new development is kicked off, we can reach it at https://stage.blog.laobian.me. Having said that, it was disabled by default.
- dev: This is the latest source code for development.

### Build

Clone repository to locally. You need tools like Microsoft Visual Studio 2019+, and npm, they will be used in normal development.

Before building the source, make sure you have .NET Core 3.0 installed.

```cs
dotnet --info
```

Now just start the msbuild, either in the CLI or visual studio.

For the assets configuration, you can just refer to the code...

### License

MIT.