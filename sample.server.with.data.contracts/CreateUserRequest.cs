namespace sample.server.with.data.contracts;

using System.Runtime.Serialization;

// Data contract for a user creation request
[DataContract]
public class CreateUserRequest
{
    [DataMember]
    public string UserName { get; set; }

    [DataMember]
    public string Email { get; set; }
}

// Data contract for a user creation response
[DataContract]
public class CreateUserResponse
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public string Message { get; set; }
}
