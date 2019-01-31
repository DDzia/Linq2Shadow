![Logo of the project](promo.png)

# Linq2Shadow

[![Build status](https://ci.appveyor.com/api/projects/status/3vs9j6ijt9w0m1ep/branch/master?svg=true)](https://ci.appveyor.com/project/DDzia/linq2shadow/branch/master)
[![Linq2Shadow](https://img.shields.io/nuget/v/Linq2Shadow.svg)](https://www.nuget.org/packages/Linq2Shadow/)

Little ORM which provide the LINQ queries to unknown sources(views, tables, stored procedures, table-valued functions) at compilation time 🤘  
[Documentation here](https://ddzia.github.io/Linq2Shadow/index.html).

> Linq2Shadow supports **MS SQL only** today.

## Features

- [x] Known `System.Linq.IQueryable` interface
- [x] Dynamicly typed models(you don't need typed models more)
- [x] Ready pack of helpers
- [x] Good documentation

## Developing

### Software requirements

- .NET Core SDK 2.2.103
- MS SQL instance(to run tests)

### Building

A quick setup which you needed to build an assembly.

```shell
dotnet restore
dotnet build
```

### Running the tests

Few steps to run tests.

```shell
dotnet restore
dotnet test
```

### Building the documentation

Tools preparing: [Use DocFX as a command-line tool](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html#2-use-docfx-as-a-command-line-tool).

Templates directory: **\<root\>/doc_templates**  
Destination directory: **\<root\>/docs**

Build via powershell:

```shell
build-doc.ps1
```

Demo serve via command line:

```shell
docx doc_templates/docfx.json --serve
```

## Licensing

The code in this project is licensed under [MIT license](LICENSE).
