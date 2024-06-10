using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Validation;

namespace server.with.data.contracts.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
 
    private readonly Task<JsonSchema> _schema;

    public UsersController()
    {
        // Read the schema from a file or a remote schema registry
        const string filePath = "../data-contract-schema.yml"; 
        var schemaText = System.IO.File.ReadAllText(filePath);
        _schema = JsonSchema.FromFileAsync(schemaText);
    }
    
    public void EnsureSchemaAndDataIsMatched(string message)
    {
        var errors = _schema.GetAwaiter().GetResult().Validate(message);
        if (errors.Count > 0)
        {
            throw new Exception("INVALID SCHEMA Data contract validation failed at producer side.");
        }
    }
    
    [HttpPost]
    public ActionResult<CreateUserResponse> CreateUser(CreateUserRequest request)
    {
        // Serialize the request object to a JSON string
        var jsonString = System.Text.Json.JsonSerializer.Serialize(request);

        // Parse the JSON string to a JsonNode
        EnsureSchemaAndDataIsMatched(jsonString);

        return Ok(new CreateUserResponse
        {
            Success = true,
            Message = "User created successfully."
        });
    }
}