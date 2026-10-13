using System.Text.RegularExpressions;
using XunitPlaywrightAutomationDemo.Pages;

namespace XunitPlaywrightAutomationDemo.Tests.UI;

public class TestSmitherySearch : BaseUITest
{
    [Fact]
    public async Task ValidateSearchForMCPServersBar()
    {
        var homePage = new HomePage(Page);

        await homePage.GoToUrlAsync();
        await Expect(Page).ToHaveTitleAsync(new Regex("Smithery - Connect agents to services in minutes"));
        await homePage.SearchAsync("Hugging Face");
        await homePage.ClickEnterKeyFromKeyboards();
        await homePage.VerifyExpectedTextInTheSearchResults("Hugging Face");
    }
}
