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

**API test output** — visible in the HTML report by clicking on an API test name.

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

## Adding tests

**New UI test:**
1. Add a page object in `Pages/` extending `BasePage`
2. Add a test class in `Tests/UI/` extending `BaseTest`

**New API test:**
1. Add a client in `ApiClients/` accepting `IAPIRequestContext` and `ITestOutputHelper`
2. Add a test class in `Tests/API/` extending `BaseApiTest`

## CI / CD

The GitHub Actions pipeline triggers on push and pull requests to `main`/`master`. It runs in headless mode using Playwright's bundled Chromium (no Chrome install needed). Test results and traces are uploaded as workflow artifacts with a 30-day retention.
