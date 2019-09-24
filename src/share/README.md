### Introduction

Shared library for private usage only.

The reason it is created mainly due to:

- A lot of codes are commonly used by differenct projects, such as [blog](https://github.com/JerryBian/laobian-blog), [jarvis](https://github.com/JerryBian/laobian-jarvis).
- .NET global tool can not reference dependency as project, nuget package can solve this issue.

### Platform

This library is built on .Net Standard 2.0, and only support latest frameworks and libraries.

Essentially Windows and Ubuntu are fully tested against, macOS should be green zone too, not guaranteed though.

### Usage

Package Manager

```sh
Install-Package Laobian.Common
```

.NET CLI

```sh
dotnet add package Laobian.Common
```

### License

```
MIT License

Copyright (c) 2018 Jerry Bian

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```