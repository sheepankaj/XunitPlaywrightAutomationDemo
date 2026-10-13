using Microsoft.Playwright;

namespace XunitPlaywrightAutomationDemo.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public async Task NavigateAsync(string url) => await Page.GotoAsync(url);
}
