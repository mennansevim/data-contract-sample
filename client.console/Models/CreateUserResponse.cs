using System.Runtime.Serialization;
namespace server.with.data.contracts.Controllers;

[DataContract]
public class CreateUserResponse
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public string Message { get; set; }
}