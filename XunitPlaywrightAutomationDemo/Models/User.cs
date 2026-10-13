using System.Text.Json.Serialization;

namespace XunitPlaywrightAutomationDemo.Models;

public record User(
    [property: JsonPropertyName("id")]       int Id,
    [property: JsonPropertyName("name")]     string Name,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("email")]    string Email
);
