using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.AspNetCore.Mvc;
namespace server.with.data.contracts.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
 
    private readonly JsonSchema _schema;

    public UsersController()
    {
        // Read the schema from a file or a remote schema registry
        const string filePath = "../data-contract-schema.json"; 
        var schemaText = System.IO.File.ReadAllText(filePath);
        _schema = JsonSchema.FromText(schemaText);
    }
    
    [HttpPost]
    public ActionResult<CreateUserResponse> CreateUser(CreateUserRequest request)
    {
        // Serialize the request object to a JSON string
        var jsonString = System.Text.Json.JsonSerializer.Serialize(request);

        // Parse the JSON string to a JsonNode
        var jsonNode = JsonNode.Parse(jsonString);
        var result = _schema.Evaluate(jsonNode, new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });
        if (!result.IsValid)
            throw new Exception("INVALID SCHEMA"); 
        
        return Ok(new CreateUserResponse
        {
            Success = true,
            Message = "User created successfully."
        });
    }
}