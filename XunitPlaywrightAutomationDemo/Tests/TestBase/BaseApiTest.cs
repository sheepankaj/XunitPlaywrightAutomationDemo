using Microsoft.Playwright;
using Microsoft.Playwright.Xunit.v3;

namespace XunitPlaywrightAutomationDemo.Tests;

public abstract class BaseApiTest : PlaywrightTest
{
    protected IAPIRequestContext RequestContext = null!;
    protected ITestOutputHelper Output => TestContext.Current.TestOutputHelper!;

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        RequestContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://jsonplaceholder.typicode.com"
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await RequestContext.DisposeAsync();
        await base.DisposeAsync();
    }
}
