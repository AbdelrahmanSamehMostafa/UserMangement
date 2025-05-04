namespace UserManagment.Application.Exceptions;

public class UserRegistrationFailedException : Exception
{
    
    public UserRegistrationFailedException(string message) : base(message)
    {
    }

   
}