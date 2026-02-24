using System.Net.Http.Json;
using System.Text.Json;

// Setting up HttpClient
using var client = new HttpClient();
var baseUrl = "http://localhost:5251/api/v1"; // Corrected port from logs

try {
    // 1. Login as admin
    Console.WriteLine("Logging in as admin...");
    var loginResponse = await client.PostAsJsonAsync($"{baseUrl}/auth/login", new { Email = "admin@school.com", Password = "Admin123!" });
    if (!loginResponse.IsSuccessStatusCode) {
        Console.WriteLine($"Login failed: {loginResponse.StatusCode}");
        var errorContent = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return;
    }
    var loginData = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
    var token = loginData.GetProperty("token").GetString();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    // 2. Create new entity
    Console.WriteLine("Creating 'Products' entity...");
    var newEntity = new {
        name = "products",
        displayName = "Products",
        displayNameSingular = "Product",
        entityTypeName = "Product",
        icon = "shopping_cart",
        fields = new[] {
            new { name = "Id", fieldType = "string", isRequired = true },
            new { name = "Name", fieldType = "string", isRequired = true },
            new { name = "Price", fieldType = "decimal", isRequired = true },
            new { name = "Stock", fieldType = "int", isRequired = false }
        }
    };

    var response = await client.PostAsJsonAsync($"{baseUrl}/discovery/entities", newEntity);
    var result = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode) {
        Console.WriteLine("Product entity created successfully!");
        Console.WriteLine(result);
    } else {
        Console.WriteLine($"Failed to create entity: {response.StatusCode}");
        Console.WriteLine(result);
    }
} catch (Exception ex) {
    Console.WriteLine($"An error occurred: {ex.Message}");
}
