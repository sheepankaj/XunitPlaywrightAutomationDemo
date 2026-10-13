using Microsoft.Playwright.Xunit.v3;

namespace XunitPlaywrightAutomationDemo.Tests;

public abstract class BaseUITest : PageTest
{
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    public override async ValueTask DisposeAsync()
    {
        // Walk up from bin/Debug/net10.0 to the project root
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var tracesDir = Path.Combine(projectRoot, "TestResults", "Traces");
        Directory.CreateDirectory(tracesDir);

        var fullName = TestContext.Current.Test?.TestDisplayName ?? "unknown";
        var testName = fullName.Contains('.')
            ? fullName[(fullName.LastIndexOf('.') + 1)..]
            : fullName;

        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(tracesDir, $"{testName}.zip")
        });

        await base.DisposeAsync();
    }
}
