# XUnit Playwright Automation Framework

A .NET test automation framework built with xUnit v3 and Playwright, following the Page Object Model (POM) design pattern. Covers both UI (browser) and API test layers.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) (`pwsh`)
- Google Chrome (for local headed runs)

## Setup

```powershell
# 1. Clone the repo
git clone <repo-url>
cd XunitPlaywrightAutomationDemo

# 2. Restore packages
dotnet restore

# 3. Build
dotnet build

# 4. Install Playwright browsers (one-time)
pwsh XunitPlaywrightAutomationDemo/bin/Debug/net10.0/playwright.ps1 install chromium
```

## Running tests

```powershell
# All tests — headed Chrome (local)
dotnet test

# All tests — headless (CI mode)
dotnet test --settings XunitPlaywrightAutomationDemo/test.ci.runsettings

# UI tests only
dotnet test --filter "FullyQualifiedName~Tests.UI"

# API tests only
dotnet test --filter "FullyQualifiedName~Tests.API"
```

## Viewing results

**HTML report** — generated at `XunitPlaywrightAutomationDemo/TestResults/TestResults.html` after each run.

**Playwright Trace Viewer** (UI tests) — step-by-step DOM snapshots, screenshots, and network:

```powershell
pwsh XunitPlaywrightAutomationDemo/bin/Debug/net10.0/playwright.ps1 show-trace `
  XunitPlaywrightAutomationDemo/TestResults/Traces/ValidateSearchForMCPServersBar.zip
```

Or drag any `.zip` from `TestResults/Traces/` to [trace.playwright.dev](https://trace.playwright.dev).

**API test output** — the VSTest HTML report only shows pass/fail. To see the step-by-step request/response logs written via `ITestOutputHelper`, use one of:

- **Terminal** — run with `--logger "console;verbosity=detailed"` to see output inline
- **GitHub Actions** — the workflow already passes `--logger "console;verbosity=detailed"`, so output appears in the Run Tests step log
- **Visual Studio Test Explorer** — click a test → View Output in the output pane

## Project structure

```
XunitPlaywrightAutomationDemo/
  Pages/                        # Page Object Model — one class per page
    BasePage.cs
    HomePage.cs
  Models/                       # Typed records for API response deserialization
    User.cs
  ApiClients/                   # API layer (POM equivalent for HTTP)
    UsersApiClient.cs
  Tests/
    TestBase/
      BaseUITest.cs             # UI base: Playwright tracing per test
      BaseApiTest.cs            # API base: IAPIRequestContext + logging
    UI/
      TestSmitherySearch.cs
    API/
      TestUsersApi.cs
  test.runsettings              # Local: headed Chrome
  test.ci.runsettings           # CI: headless Chromium
.github/
  workflows/
    playwright-tests.yml        # GitHub Actions pipeline
```

## NuGet package dependency guide

### Critical: use `xunit.v3`, NOT `xunit`

If you add packages through the NuGet Package Manager UI or `dotnet add package`, the default search result for "xunit" returns `xunit` (v2). **Do not install that package.** It causes this build error:

```
error CS0433: The type 'FactAttribute' exists in both
  'xunit.core, Version=2.x.x' and 'xunit.v3.core, Version=1.x.x'
```

This happens because `Microsoft.Playwright.Xunit.v3` already pulls in `xunit.v3.core` internally. Adding `xunit` (v2) on top of it creates a type conflict that breaks the build.

**Always use `xunit.v3` (the v3 package) for this project.**

### Package versions (managed centrally in `Directory.Packages.props`)

| Package | Version | Notes |
|---|---|---|
| `xunit.v3` | 1.0.1 | xUnit v3 core — **not** `xunit` (v2) |
| `Microsoft.Playwright.Xunit.v3` | 1.63.0 | Playwright xUnit v3 adapter |
| `xunit.runner.visualstudio` | 3.1.4 | Must be v3.x to match xUnit v3 |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | VSTest host |
| `coverlet.collector` | 6.0.4 | Code coverage |

All package versions are defined once in `Directory.Packages.props` at the solution root. Do not add `Version="..."` attributes in the `.csproj` — Central Package Management will reject it.

---

## Allure Reports — current status with xUnit v3

### Why Allure does not work right now

[Allure](https://allurereport.org) is free and open-source (Apache 2.0). The NuGet package `Allure.Xunit` works well with xUnit v2 projects, but **it is not compatible with xUnit v3** as of this writing.

The root cause is the same type-conflict pattern: `Allure.Xunit` (all published versions, including pre-releases) has a hard dependency on `xunit.core` v2, which conflicts with `xunit.v3.core`:

```
error CS0433: The type 'FactAttribute' exists in both
  'xunit.core, Version=2.4.1' and 'xunit.v3.core, Version=1.0.1'
```

This is not a configuration issue — it cannot be worked around without the Allure team shipping a proper xUnit v3 adapter.

### What to use instead (today)

- **UI tests** → Playwright Trace Viewer — gives the same step-by-step experience as the TypeScript Playwright HTML reporter (DOM snapshots, screenshots, network requests, timeline). See [Viewing results](#viewing-results) above.
- **API tests** → `ITestOutputHelper` — logs steps and request/response detail; visible in the terminal, GitHub Actions run log, and Visual Studio Test Explorer (not in the VSTest HTML report).

### When Allure v3 support lands

Track progress on the [Allure.Net GitHub repository](https://github.com/allure-framework/allure-csharp). Once a version ships that does not depend on `xunit.core` v2, adding it will be a two-step change:

1. Add the new package to `Directory.Packages.props` and `.csproj`
2. Add `[AllureSuite]` / `[AllureFeature]` / `[AllureStory]` attributes to test classes and `AllureApi.Step()` calls inside page object methods

No changes to `BaseUITest`, `BaseApiTest`, or the test structure will be required.

---

## Adding tests

**New UI test:**
1. Add a page object in `Pages/` extending `BasePage`
2. Add a test class in `Tests/UI/` extending `BaseTest`

**New API test:**
1. Add a client in `ApiClients/` accepting `IAPIRequestContext` and `ITestOutputHelper`
2. Add a test class in `Tests/API/` extending `BaseApiTest`

## CI / CD

The GitHub Actions pipeline triggers on push and pull requests to `main`/`master`. It runs in headless mode using Playwright's bundled Chromium (no Chrome install needed). Test results and traces are uploaded as workflow artifacts with a 30-day retention.
