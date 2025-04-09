namespace Application.Users.Exceptions;

public class UserException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class UserNotFoundException(Guid id) 
    : UserException(id, $"User under id: {id} not found!");

public class UserIdNotFoundException() 
    : UserException(Guid.Empty, $"User id not found!");

public class UserRoleNotFoundException() 
    : UserException(Guid.Empty, $"User role not found!");

public class UserAlreadyRegisteredException(Guid id) 
    : UserException(id, $"User under id: {id} is already registered on this course!");

public class UserWithNameAlreadyExistsException(Guid id) 
    : UserException(id, $"User under such user name already exists!");

public class UserWithEmailAlreadyExistsException(Guid id)
    : UserException(id, $"User under such email already exists!");

public class EmailNotVerifiedException(Guid id)
    : UserException(id, $"User email is not verified!");

public class InvalidVerificationTokenException(Guid id)
    : UserException(id, $"Invalid verification token!");

public class EmailVerificationTokenExpiredException(Guid id)
    : UserException(id, $"Email verification token expired!");


public class InvalidCredentialsException() 
    : UserException(Guid.Empty, $"Invalid credentials!");

public class UserUnauthorizedAccessException(string message) 
    : UserException(Guid.Empty, message);

public class UserUnknownException(Guid id, Exception innerException)
    : UserException(id, $"Unknown exception for the User under id: {id}!", innerException);