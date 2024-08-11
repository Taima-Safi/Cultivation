namespace FourthPro.Shared.Exception;

public class AlreadyExistException : System.Exception
{
    public AlreadyExistException(string message, object detailedErrorMessage = null) : base(message)
    {
        if (detailedErrorMessage != null)
            Data.Add("custom_message", detailedErrorMessage);
    }
}
