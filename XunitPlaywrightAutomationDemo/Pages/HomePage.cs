using Microsoft.Playwright;

namespace XunitPlaywrightAutomationDemo.Pages;

public class HomePage : BasePage
{
    private const string Url = "https://smithery.ai/";
    private ILocator MCPServersSearchBar => Page.Locator("#hero-server-search");
    private ILocator resultsTable => Page.Locator("//div[@class='space-y-3']");

    public HomePage(IPage page) : base(page) { }

    public async Task GoToUrlAsync() => await NavigateAsync(Url);

    public async Task SearchAsync(string query) => await MCPServersSearchBar.FillAsync(query);
    public async Task ClickEnterKeyFromKeyboards() => await MCPServersSearchBar.PressAsync("Enter");
    public async Task VerifyExpectedTextInTheSearchResults(string expectedText) =>
        await Assertions.Expect(resultsTable).ToContainTextAsync(expectedText);

}
