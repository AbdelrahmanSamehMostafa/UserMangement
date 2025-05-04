namespace UserManagment.Common.Helpers
{

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }
    }
}
