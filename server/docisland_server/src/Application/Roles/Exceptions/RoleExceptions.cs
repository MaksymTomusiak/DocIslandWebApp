namespace Application.Roles.Exceptions;

public class RoleException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class RoleNotFoundException(Guid id)
    : RoleException(id, $"Role under id: {id} not found!");

public class RoleNameAlreadyExistsException(Guid id, string name)
    : RoleException(id, $"Role under such name: {name} already exists!");

public class RoleUnknownException(Guid id, Exception innerException)
    : RoleException(id, $"Unknown exception for the Role under id: {id}!", innerException);
    
public class UserIdNotFoundException()
: RoleException(Guid.Empty, $"User id not found!");

public class UserNotFoundException(Guid id) 
    : RoleException(id, $"User under id: {id} not found!");
    
public class UserUnauthorizedAccessException(string message) 
    : RoleException(Guid.Empty, message);
        