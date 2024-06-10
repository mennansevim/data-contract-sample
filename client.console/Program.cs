using System.Net;
using System.Text;
using System.Text.Json;
using server.with.data.contracts.Controllers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace client.console;

class Program
{
    [Obsolete("Obsolete")]
    static async Task Main(string[] args)
    {
        const string yamlUrl =
            "https://gist.githubusercontent.com/mennansevim/72f90446c31b8e38de99f18dd031ffb7/raw/52f0ebe9a6bdaf7457868788006686573671801d/datacontract.yaml";
        var yamlContent = await FetchRawData(yamlUrl);
        var jsonSchema = ConvertYamlToJsonSchema(yamlContent);

        using var client = new HttpClient();
        try
        {
            var jsonData = JsonSerializer.Serialize(new CreateUserRequest
            {
                UserName = "Mennan Sevim",
                Email = "mennansevim@gmail.com",
                IsActivec = false
            });

            // Validate your data and remote yaml(as json) schema
            if (!IsValidJson(jsonData, jsonSchema))
                throw new Exception("INVALID SCHEMA");

            var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

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

    public static async Task<string> FetchRawData(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
                return null;
            }
        }
    }

    static string ConvertYamlToJsonSchema(string yamlContent)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlObject = deserializer.Deserialize(new System.IO.StringReader(yamlContent));

        var serializer = new SerializerBuilder()
            .JsonCompatible()
            .Build();

        return serializer.Serialize(yamlObject);
    }

    [Obsolete("Obsolete")]
    private static bool IsValidJson(string jsonData, string jsonSchema)
    {
        var jsonObject = JObject.Parse(jsonData);
        JsonSchema schema = JsonSchema.Parse(jsonSchema);

        return jsonObject.IsValid(schema, out var errorMessages);
    }
}