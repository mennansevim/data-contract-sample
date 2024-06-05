using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using server.with.data.contracts.Controllers;

namespace client.console;

class Program
{
    static async Task Main(string[] args)
    {
        const string filePath = "../../../../data-contract-schema.json"; 
        var schemaText = System.IO.File.ReadAllText(filePath);
        var schema = JsonSchema.FromText(schemaText);
      
        
        using var client = new HttpClient();
        try
        {
            var jsonRequest = JsonSerializer.Serialize(new CreateUserRequest
            {
                UserName = "Mennan Sevim",
                Email = "mennansevim@gmail.com",
                IsActive = false
            });

            // Parse the JSON string to a JsonNode
            var jsonNode = JsonNode.Parse(jsonRequest);
            
            // Validate request before send it
            var result = schema.Evaluate(jsonNode, new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });
            if (!result.IsValid)
                throw new Exception("INVALID SCHEMA"); 
                
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
         
            // Define the local endpoint URL
            var url = "https://localhost:7155/api/Users";
                
            // Make the GET request
            var response = await client.PostAsync(url, httpContent);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            var responseBody = await response.Content.ReadAsStringAsync();

            // Output the response to the console
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            // Handle exception
            Console.WriteLine($"Request error: {e.Message}");
        }
    }
}