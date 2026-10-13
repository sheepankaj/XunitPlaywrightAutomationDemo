using System.Text.Json;
using Microsoft.Playwright;
using Xunit;
using XunitPlaywrightAutomationDemo.Models;

namespace XunitPlaywrightAutomationDemo.ApiClients;

public class UsersApiClient
{
    private readonly IAPIRequestContext _request;
    private readonly ITestOutputHelper _output;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public UsersApiClient(IAPIRequestContext request, ITestOutputHelper output)
    {
        _request = request;
        _output = output;
    }

    public async Task<IAPIResponse> GetUsersResponseAsync()
    {
        _output.WriteLine("→ GET /users");
        var response = await _request.GetAsync("/users");
        _output.WriteLine($"← {response.Status} {response.StatusText}");
        return response;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await GetUsersResponseAsync();
        var json = await response.TextAsync();
        _output.WriteLine($"Body (truncated): {json[..Math.Min(200, json.Length)]}...");
        return JsonSerializer.Deserialize<List<User>>(json, JsonOptions)!;
    }
}
