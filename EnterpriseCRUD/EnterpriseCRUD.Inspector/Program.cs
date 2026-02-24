using System.Net.Http.Json;
using System.Text.Json;

var httpClient = new HttpClient();

Console.WriteLine("Logging in...");
// Login to get token
var loginResp = await httpClient.PostAsJsonAsync("http://localhost:5251/api/v1/auth/login", new {
    email = "admin@school.com",
    password = "Admin123!"
});

if (!loginResp.IsSuccessStatusCode) {
    Console.WriteLine($"Login failed: {loginResp.StatusCode}");
    return;
}

var loginData = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
var token = loginData.GetProperty("token").GetString();

httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

Console.WriteLine("Fetching students...");
// Fetch students
var resp = await httpClient.GetAsync("http://localhost:5251/api/v1/students?_page=1&_perPage=5");
var content = await resp.Content.ReadAsStringAsync();

Console.WriteLine("--- API Response for /api/v1/students ---");
var options = new JsonSerializerOptions { WriteIndented = true };
var json = JsonSerializer.Deserialize<JsonElement>(content);
Console.WriteLine(JsonSerializer.Serialize(json, options));

// Also check headers
Console.WriteLine("\n--- Headers ---");
foreach (var header in resp.Headers)
{
    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
}
