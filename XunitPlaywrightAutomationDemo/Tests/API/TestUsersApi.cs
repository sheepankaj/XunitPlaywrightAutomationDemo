using Microsoft.Playwright;
using XunitPlaywrightAutomationDemo.ApiClients;

namespace XunitPlaywrightAutomationDemo.Tests.API;

public class TestUsersApi : BaseApiTest
{
    [Fact]
    public async Task ValidateUserWithId()
    {
        var client = new UsersApiClient(RequestContext, Output);

        Output.WriteLine("Step 1: Send GET /users and assert 200 OK");
        var response = await client.GetUsersResponseAsync();
        await Assertions.Expect(response).ToBeOKAsync();

        Output.WriteLine("Step 2: Deserialise response body");
        var users = await client.GetUsersAsync();

        Output.WriteLine("Step 3: Find user with id=2");
        var user = users.Find(u => u.Id == 2);
        Assert.NotNull(user);

        Output.WriteLine($"Step 4: Assert fields — name={user.Name}, username={user.Username}, email={user.Email}");
        Assert.Equal("Ervin Howell",      user.Name);
        Assert.Equal("Antonette",         user.Username);
        Assert.Equal("Shanna@melissa.tv", user.Email);

        Output.WriteLine("All assertions passed.");
    }
}
