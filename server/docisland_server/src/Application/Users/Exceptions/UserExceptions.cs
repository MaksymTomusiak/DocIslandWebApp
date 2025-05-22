namespace Application.Users.Exceptions;

public class UserException(string id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public string Id { get; } = id;
}

public class UserNotFoundException(string id) 
    : UserException(id, $"User under id: {id} not found!");

public class UserIdNotFoundException() 
    : UserException(string.Empty, $"User id not found!");

public class UserRoleNotFoundException() 
    : UserException(string.Empty, $"User role not found!");

public class UserAlreadyRegisteredException(string id) 
    : UserException(id, $"User under id: {id} is already registered on this course!");

public class UserWithNameAlreadyExistsException(string id) 
    : UserException(id, $"User under such user name already exists!");

public class UserWithEmailAlreadyExistsException(string id)
    : UserException(id, $"User under such email already exists!");

public class EmailNotVerifiedException(string id)
    : UserException(id, $"User email is not verified!");

public class InvalidVerificationTokenException(string id)
    : UserException(id, $"Invalid verification token!");

public class EmailVerificationTokenExpiredException(string id)
    : UserException(id, $"Email verification token expired!");

public class InvalidCredentialsException() 
    : UserException(string.Empty, $"Invalid credentials!");

public class UserUnauthorizedAccessException(string message) 
    : UserException(string.Empty, message);

public class UserUnknownException(string id, Exception innerException)
    : UserException(id, $"Unknown exception for the User under id: {id}!", innerException);