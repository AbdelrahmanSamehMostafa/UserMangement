namespace UserManagment.Application.Exceptions;

public class LoginFailedException : Exception
{
    

    public LoginFailedException(string message, Exception innerException) : base(message, innerException)
    {

    }
}