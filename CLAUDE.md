# XUnit Playwright Automation Framework

## Stack
- **.NET 10** — target framework
- **xUnit v3** (`xunit.v3` 1.0.1) — test framework; use `xunit.v3` not `xunit` (v2 causes type conflicts)
- **Microsoft.Playwright.Xunit.v3** (1.63.0) — Playwright integration for xUnit v3
- **Central Package Management** — all versions live in `Directory.Packages.props`, never in the `.csproj`

## Project structure

```
XunitPlaywrightAutomationDemo/          ← project root
  Pages/
    BasePage.cs                         ← abstract base for all page objects
    HomePage.cs                         ← Smithery home page
  Models/
    User.cs                             ← typed records for API response deserialization
  ApiClients/
    UsersApiClient.cs                   ← API client (POM equivalent for API layer)
  Tests/
    TestBase/
      BaseUITest.cs                     ← UI base: starts/stops Playwright tracing per test
      BaseApiTest.cs                    ← API base: sets up IAPIRequestContext + ITestOutputHelper
    UI/
      TestSmitherySearch.cs
    API/
      TestUsersApi.cs
  test.runsettings                      ← local: headed Chrome
  test.ci.runsettings                   ← CI: headless bundled Chromium, TRX + HTML loggers
.github/
  workflows/
    playwright-tests.yml                ← GitHub Actions CI pipeline
```

## Running tests

```powershell
# Local (headed Chrome, picks up test.runsettings automatically via csproj)
dotnet test

# CI / headless
dotnet test --settings XunitPlaywrightAutomationDemo/test.ci.runsettings
```

## Viewing Playwright traces (UI tests only)

After a local run, traces land at `XunitPlaywrightAutomationDemo/TestResults/Traces/<TestName>.zip`.

```powershell
pwsh ".\XunitPlaywrightAutomationDemo\bin\Debug\net10.0\playwright.ps1" show-trace ".\XunitPlaywrightAutomationDemo\TestResults\Traces\ValidateSearchForMCPServersBar.zip"
```

Or drag the `.zip` to https://trace.playwright.dev.

## Architecture decisions

### POM layers
- **UI tests** → `Pages/` — extend `BasePage`, expose async methods, use `ILocator` properties
- **API tests** → `ApiClients/` — take `IAPIRequestContext` + `ITestOutputHelper` in constructor
- **Test classes** extend `BaseUITest` (UI) or `BaseApiTest` (API), never `PageTest`/`PlaywrightTest` directly

### Reporting
- **UI** — Playwright trace viewer (step-by-step DOM snapshots, screenshots, network)
- **API** — `ITestOutputHelper` output visible in the HTML report per test
- **Allure.Xunit is incompatible with xUnit v3** — it pulls in `xunit.core` v2 which causes a `FactAttribute` type conflict; do not add it until the Allure team ships a v3 adapter

### runsettings split
- `test.runsettings` — `Headless=false`, `Channel=chrome` (requires Chrome installed locally)
- `test.ci.runsettings` — `Headless=true`, no Channel (uses bundled Playwright Chromium)
- `RunSettingsFilePath` in the `.csproj` auto-applies the local runsettings for `dotnet test`

## Adding a new page object

1. Create `Pages/FooPage.cs` extending `BasePage`
2. Declare locators as `private ILocator` properties
3. Expose async methods for each user action
4. Add a test class in `Tests/UI/` extending `BaseTest`

## Adding a new API client

1. Create `ApiClients/FooApiClient.cs` accepting `IAPIRequestContext` and `ITestOutputHelper`
2. Add a test class in `Tests/API/` extending `BaseApiTest`
3. Use `Output.WriteLine()` to log request/response steps
