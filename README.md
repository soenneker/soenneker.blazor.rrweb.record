[![](https://img.shields.io/nuget/v/soenneker.blazor.rrweb.record.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.rrweb.record/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.rrweb.record/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.rrweb.record/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.rrweb.record.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.rrweb.record/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.rrweb.record)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.rrweb.record/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.rrweb.record/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.Rrweb.Record
### A Blazor interop library for recording browser sessions with rrweb.

## Installation

```bash
dotnet add package Soenneker.Blazor.Rrweb.Record
```

## Setup

Register services in `Program.cs`:

```csharp
builder.Services.AddRrwebRecordInteropAsScoped();
```

Inject the higher-level utility where you need it:

```csharp
@inject IRrwebRecordInterop Record
```

## Usage

Initialize the package once before first use:

```csharp
await Record.Initialize();
```
