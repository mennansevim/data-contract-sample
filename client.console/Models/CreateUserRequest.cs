using System.Runtime.Serialization;
namespace server.with.data.contracts.Controllers;

[DataContract]
public class CreateUserRequest
{
    [DataMember]
    public string UserName { get; set; }

    [DataMember]
    public string Email { get; set; }
    
    [DataMember]
    public bool IsActivec { get; set; }
}