namespace Application.Roles.Exceptions;

public class RoleException(string id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public string Id { get; } = id;
}

public class RoleNotFoundException(string id)
    : RoleException(id, $"Role under id: {id} not found!");

public class RoleNameAlreadyExistsException(string id, string name)
    : RoleException(id, $"Role under such name: {name} already exists!");

public class RoleUnknownException(string id, Exception innerException)
    : RoleException(id, $"Unknown exception for the Role under id: {id}!", innerException);
    
public class UserIdNotFoundException()
: RoleException(string.Empty, $"User id not found!");

public class UserNotFoundException(string id) 
    : RoleException(id, $"User under id: {id} not found!");
    
public class UserUnauthorizedAccessException(string message) 
    : RoleException(string.Empty, message);
        